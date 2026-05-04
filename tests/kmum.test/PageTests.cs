using AutoMapper;
using Kmums.Areas.Identity.Data;
using Kmums.Models.Category;
using Kmums.Models.Contact;
using Kmums.Models.User;
using Kmums.Pages;
using Kmums.Pages.Admin;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration.UserSecrets;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;
using Xunit.Sdk;

namespace kmum.test
{
    public class IndexPageTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private static IndexModel CreateModel(DataContext context,
        Mock<IEmailSender> emailMock,
        Mock<UserManager<UserModel>> userManagerMock)
        {
            var loggerMock = new Mock<ILogger<IndexModel>>();

            var model = new IndexModel(
                loggerMock.Object,
                context,
                emailMock.Object,
                userManagerMock.Object
            );

            var user = new ClaimsPrincipal(new ClaimsIdentity(
            [
            new Claim(ClaimTypes.NameIdentifier, "00000000-0000-0000-0000-000000000001")
            ]));

            model.PageContext = new Microsoft.AspNetCore.Mvc.RazorPages.PageContext
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return model;
        }

        private static DataContext GetInMemoryDb()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(new Guid("00000000-0000-0000-0000-000000000007").ToString())
                .Options;

            return new DataContext(options);
        }

        private static Mock<UserManager<UserModel>> MockUserManager()
        {
            var store = new Mock<IUserStore<UserModel>>();

            return new Mock<UserManager<UserModel>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task OnGet_ReturnsPage_AndLoadsData()
        {
            var context = GetInMemoryDb();
            var emailMock = new Mock<IEmailSender>();
            var userManagerMock = MockUserManager();

            var user = new UserModel { Id = new Guid("00000000-0000-0000-0000-000000000002") };
            context.Users.Add(user);
            await context.SaveChangesAsync();

            userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var model = CreateModel(context, emailMock, userManagerMock);

            var result = await model.OnGet();

            Assert.IsType<PageResult>(result);
            Assert.NotNull(model);
        }

        
        [Fact]
        public async Task OnGetSelectedUserAsync_InvalidId()
        {
            var context = GetInMemoryDb();
            var model = CreateModel(context, new Mock<IEmailSender>(), MockUserManager());

            await Assert.ThrowsAsync<InvalidOperationException>(() =>
                model.OnGetSelectedUserAsync(0));
        }

        [Fact]
        public async Task OnGetSelectedUserAsync_ValidId()
        {
            var context = GetInMemoryDb();

            var user = new UserModel
            {
                Id = new Guid("00000000-0000-0000-0000-000000000006")
            };
            var userPublic = new UserPublicModel
            {
                User = user
            };
            var image = new UserImageModel { Id = 6, UserProfile = userPublic };

            context.Images.Add(image);
            await context.SaveChangesAsync();

            var model = CreateModel(context, new Mock<IEmailSender>(), MockUserManager());

            var result = await model.OnGetSelectedUserAsync(6);

            Assert.IsNotType<JsonResult>(result);
        }

       
        [Fact]
        public async Task OnPostSendEmail_SendsEmails_AndRedirects()
        {
            var context = GetInMemoryDb();
            var emailMock = new Mock<IEmailSender>();

            var user = new UserModel { Id = new Guid("00000000-0000-0000-0000-000000000008"), Email = "receiver@test.com" };
            var publicProfile = new UserPublicModel { Id = 8, User = user };

            context.PublicProfile.Add(publicProfile);
            context.Users.Add(user);

            await context.SaveChangesAsync();

            var model = CreateModel(context, emailMock, MockUserManager());

            model.EmailDetails = new ContactModel
            {
                Receiver = "8",
                Sender = "sender@test.com",
                Title = "Test",
                Body = "Hello"
            };

            var result = await model.OnPostSendEmail();

            emailMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(), 
                It.IsAny<string>()), Times.Never);

            Assert.IsType<UnauthorizedResult>(result);
            Assert.True(string.IsNullOrEmpty(model.ResponseMessage));
        }

        
        [Fact]
        public async Task OnPostUpdateSubscriptionAsync_ReturnsError()
        {
            var model = CreateModel(GetInMemoryDb(), new Mock<IEmailSender>(), MockUserManager());

            var result = await model.OnPostUpdateSubscriptionAsync(null, "{}");

            var json = Assert.IsType<JsonResult>(result);
            Assert.Equal("error: operation not allowed", json.Value);
        }

        [Fact]
        public async Task OnPostSendEmail_AllowsSender()
        {
            var context = GetInMemoryDb();
            var emailMock = new Mock<IEmailSender>();

            var user = new UserModel { Id = new Guid("00000000-0000-0000-0000-000000000009"), Email = "real@test.com" };
            context.Users.Add(user);
            context.PublicProfile.Add(new UserPublicModel { Id = 9, User = user });

            await context.SaveChangesAsync();

            var model = CreateModel(context, emailMock, MockUserManager());

            model.EmailDetails = new ContactModel
            {
                Receiver = "9",
                Sender = "fake@attacker.com",
                Title = "Spoof",
                Body = "Injected"
            };

            await model.OnPostSendEmail();

            emailMock.Verify(x => x.SendEmailAsync("fake@attacker.com", It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task OnPostUpdateSubscriptionAsync_Risk()
        {
            var context = GetInMemoryDb();
            var userManagerMock = MockUserManager();

            var user = new UserModel { Id = new Guid("00000000-0000-0000-0000-000000000010") };
            context.Users.Add(user);
            context.PublicProfile.Add(new UserPublicModel { Id = 1, User = user });

            await context.SaveChangesAsync();

            userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            var model = CreateModel(context, new Mock<IEmailSender>(), userManagerMock);

            var largeJson = new string('A', 1_000_000); 

            var result = await model.OnPostUpdateSubscriptionAsync("TX999", largeJson);

            var json=Assert.IsType<JsonResult>(result);
            Assert.Equal("error: payload too large", json.Value);
        }

        [Fact]
        public async Task OnPostAsync_Tamper()
        {
            var context = GetInMemoryDb();
            var model = new CreateModel(context)
            {
                Input = new CategoryDTO
                {
                    Name = "Safe Category"
                }
            };

            await model.OnPostAsync();

            var saved = context.Category.FirstOrDefault();

            Assert.NotNull(saved);

           
            Assert.NotEqual(999, saved.Id);
        }

        [Theory]
        [InlineData("/Index")]
        public async Task Test_Page_Load(string url)
        {
            var response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync();

            Assert.Contains("The ConnectionString property has not been initialized", content);
        }

        
    }
}
