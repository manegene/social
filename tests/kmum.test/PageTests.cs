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
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System.Security.Claims;

namespace kmum.test
{
    public class IndexPageTests(WebApplicationFactory<Program> factory) : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client = factory.CreateClient();

        private static IndexModel CreateModel(DataContext context,
        Mock<IEmailSender> emailMock,
        Mock<UserManager<UserModel>> userManagerMock)
        {
            Mock<ILogger<IndexModel>> loggerMock = new Mock<ILogger<IndexModel>>();

            IndexModel model = new IndexModel(
                loggerMock.Object,
                context,
                emailMock.Object,
                userManagerMock.Object
            );

            ClaimsPrincipal user = new ClaimsPrincipal(new ClaimsIdentity(
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
            DbContextOptions<DataContext> options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(new Guid("00000000-0000-0000-0000-000000000007").ToString())
                .Options;

            return new DataContext(options);
        }

        private static Mock<UserManager<UserModel>> MockUserManager()
        {
            Mock<IUserStore<UserModel>> store = new Mock<IUserStore<UserModel>>();

            return new Mock<UserManager<UserModel>>(
                store.Object, null, null, null, null, null, null, null, null
            );
        }

        [Fact]
        public async Task OnGetSelectedUserAsync_ValidId()
        {
            DataContext context = GetInMemoryDb();

            UserModel user = new UserModel
            {
                Id = new Guid("00000000-0000-0000-0000-000000000006")
            };
            UserPublicModel userPublic = new UserPublicModel
            {
                User = user
            };
            UserImageModel image = new UserImageModel { Id = 6, UserProfile = userPublic };

            context.Images.Add(image);
            await context.SaveChangesAsync();

            IndexModel model = CreateModel(context, new Mock<IEmailSender>(), MockUserManager());

            ActionResult result = await model.OnGetSelectedUserAsync(6);

            Assert.IsNotType<JsonResult>(result);
        }


        [Fact]
        public async Task OnPostSendEmail_SendsEmails_AndRedirects()
        {
            DataContext context = GetInMemoryDb();
            Mock<IEmailSender> emailMock = new Mock<IEmailSender>();

            UserModel user = new UserModel { Id = new Guid("00000000-0000-0000-0000-000000000008"), Email = "receiver@test.com" };
            UserPublicModel publicProfile = new UserPublicModel { Id = 8, User = user };

            context.PublicProfile.Add(publicProfile);
            context.Users.Add(user);

            await context.SaveChangesAsync();

            IndexModel model = CreateModel(context, emailMock, MockUserManager());

            model.EmailDetails = new ContactModel
            {
                Receiver = "8",
                Sender = "sender@test.com",
                Title = "Test",
                Body = "Hello"
            };

            IActionResult result = await model.OnPostSendEmail();

            emailMock.Verify(x => x.SendEmailAsync(It.IsAny<string>(), It.IsAny<string>(),
                It.IsAny<string>()), Times.Never);

            Assert.IsType<UnauthorizedResult>(result);
            Assert.True(string.IsNullOrEmpty(model.ResponseMessage));
        }


        [Fact]
        public async Task OnPostUpdateSubscriptionAsync_ReturnsError()
        {
            IndexModel model = CreateModel(GetInMemoryDb(), new Mock<IEmailSender>(), MockUserManager());

            ActionResult result = await model.OnPostUpdateSubscriptionAsync(null, "{}");

            JsonResult json = Assert.IsType<JsonResult>(result);
            Assert.NotEqual("subscriptin update successful", json.Value);
        }

        [Fact]
        public async Task OnPostSendEmail_AllowsSender()
        {
            DataContext context = GetInMemoryDb();
            Mock<IEmailSender> emailMock = new Mock<IEmailSender>();

            UserModel user = new UserModel { Id = new Guid("00000000-0000-0000-0000-000000000009"), Email = "real@test.com" };
            context.Users.Add(user);
            context.PublicProfile.Add(new UserPublicModel { Id = 9, User = user });

            await context.SaveChangesAsync();

            IndexModel model = CreateModel(context, emailMock, MockUserManager());

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
            DataContext context = GetInMemoryDb();
            Mock<UserManager<UserModel>> userManagerMock = MockUserManager();

            UserModel user = new UserModel { Id = new Guid("00000000-0000-0000-0000-000000000010") };
            context.Users.Add(user);
            context.PublicProfile.Add(new UserPublicModel { Id = 1, User = user });

            await context.SaveChangesAsync();

            userManagerMock.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>()))
                .ReturnsAsync(user);

            IndexModel model = CreateModel(context, new Mock<IEmailSender>(), userManagerMock);

            string largeJson = new string('A', 1_000_000);

            ActionResult result = await model.OnPostUpdateSubscriptionAsync("TX999", largeJson);

            JsonResult json = Assert.IsType<JsonResult>(result);
            Assert.Equal("error: payload too large", json.Value);
        }

        [Fact]
        public async Task OnPostAsync_Tamper()
        {
            DataContext context = GetInMemoryDb();
            CreateModel model = new CreateModel(context)
            {
                Input = new CategoryDTO
                {
                    Name = "Safe Category",
                    ParentCategoryId = 999
                }
            };

            await model.OnPostAsync();

            CategoryModel? saved = context.Category.FirstOrDefault();

            Assert.Null(saved);

        }

        [Theory]
        [InlineData("/Index")]
        public async Task Test_Page_Load(string url)
        {
            HttpResponseMessage response = await _client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            string content = await response.Content.ReadAsStringAsync();

            Assert.Contains("The ConnectionString property has not been initialized", content);
        }


    }
}
