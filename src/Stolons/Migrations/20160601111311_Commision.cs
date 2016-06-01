using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace Stolons.Migrations
{
    public partial class Commision : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AspNetRoles",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    NormalizedName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRole", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "ApplicationConfig",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    Comission = table.Column<int>(nullable: false),
                    IsModeSimulated = table.Column<bool>(nullable: false),
                    MailAddress = table.Column<string>(nullable: true),
                    MailPassword = table.Column<string>(nullable: true),
                    MailPort = table.Column<int>(nullable: false),
                    MailSmtp = table.Column<string>(nullable: true),
                    OrderDayStartDate = table.Column<int>(nullable: false),
                    OrderDeliveryMessage = table.Column<string>(nullable: true),
                    OrderHourStartDate = table.Column<int>(nullable: false),
                    OrderMinuteStartDate = table.Column<int>(nullable: false),
                    PreparationDayStartDate = table.Column<int>(nullable: false),
                    PreparationHourStartDate = table.Column<int>(nullable: false),
                    PreparationMinuteStartDate = table.Column<int>(nullable: false),
                    SimulationMode = table.Column<int>(nullable: false),
                    StockUpdateDayStartDate = table.Column<int>(nullable: false),
                    StockUpdateHourStartDate = table.Column<int>(nullable: false),
                    StockUpdateMinuteStartDate = table.Column<int>(nullable: false),
                    StolonsAboutPageText = table.Column<string>(nullable: true),
                    StolonsAddress = table.Column<string>(nullable: true),
                    StolonsLabel = table.Column<string>(nullable: true),
                    StolonsPhoneNumber = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationConfig", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "ProductType",
                columns: table => new
                {
                    Name = table.Column<string>(nullable: false),
                    Image = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductType", x => x.Name);
                });
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Address = table.Column<string>(nullable: true),
                    Avatar = table.Column<string>(nullable: true),
                    City = table.Column<string>(nullable: true),
                    Cotisation = table.Column<bool>(nullable: false),
                    DisableReason = table.Column<string>(nullable: true),
                    Discriminator = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: true),
                    Enable = table.Column<bool>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PostCode = table.Column<string>(nullable: true),
                    RegistrationDate = table.Column<DateTime>(nullable: false),
                    Surname = table.Column<string>(nullable: false),
                    Area = table.Column<int>(nullable: true),
                    CompanyName = table.Column<string>(nullable: true),
                    ExploitationPicuresSerialized = table.Column<string>(nullable: true),
                    Latitude = table.Column<double>(nullable: true),
                    Longitude = table.Column<double>(nullable: true),
                    OpenText = table.Column<string>(nullable: true),
                    Production = table.Column<string>(nullable: true),
                    StartDate = table.Column<int>(nullable: true),
                    WebSiteLink = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
            migrationBuilder.CreateTable(
                name: "AspNetRoleClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityRoleClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityRoleClaim<string>_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "ProductFamilly",
                columns: table => new
                {
                    FamillyName = table.Column<string>(nullable: false),
                    Image = table.Column<string>(nullable: true),
                    TypeName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductFamilly", x => x.FamillyName);
                    table.ForeignKey(
                        name: "FK_ProductFamilly_ProductType_TypeName",
                        column: x => x.TypeName,
                        principalTable: "ProductType",
                        principalColumn: "Name",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUsers",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    AccessFailedCount = table.Column<int>(nullable: false),
                    ConcurrencyStamp = table.Column<string>(nullable: true),
                    Email = table.Column<string>(nullable: true),
                    EmailConfirmed = table.Column<bool>(nullable: false),
                    LockoutEnabled = table.Column<bool>(nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(nullable: true),
                    NormalizedEmail = table.Column<string>(nullable: true),
                    NormalizedUserName = table.Column<string>(nullable: true),
                    PasswordHash = table.Column<string>(nullable: true),
                    PhoneNumber = table.Column<string>(nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(nullable: false),
                    SecurityStamp = table.Column<string>(nullable: true),
                    TwoFactorEnabled = table.Column<bool>(nullable: false),
                    UserId = table.Column<int>(nullable: true),
                    UserName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ApplicationUser", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ApplicationUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "ConsumerBill",
                columns: table => new
                {
                    BillNumber = table.Column<string>(nullable: false),
                    ConsumerId = table.Column<int>(nullable: true),
                    EditionDate = table.Column<DateTime>(nullable: false),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ConsumerBill", x => x.BillNumber);
                    table.ForeignKey(
                        name: "FK_ConsumerBill_Consumer_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "News",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    DateOfPublication = table.Column<DateTime>(nullable: false),
                    ImageLink = table.Column<string>(nullable: true),
                    Message = table.Column<string>(nullable: false),
                    Title = table.Column<string>(nullable: false),
                    UserForeignKey = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_News", x => x.Id);
                    table.ForeignKey(
                        name: "FK_News_User_UserForeignKey",
                        column: x => x.UserForeignKey,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "ProducerBill",
                columns: table => new
                {
                    BillNumber = table.Column<string>(nullable: false),
                    EditionDate = table.Column<DateTime>(nullable: false),
                    ProducerId = table.Column<int>(nullable: true),
                    State = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProducerBill", x => x.BillNumber);
                    table.ForeignKey(
                        name: "FK_ProducerBill_Producer_ProducerId",
                        column: x => x.ProducerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "TempWeekBasket",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConsumerId = table.Column<int>(nullable: true),
                    Validated = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TempWeekBasket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TempWeekBasket_Consumer_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "ValidatedWeekBasket",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ConsumerId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ValidatedWeekBasket", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ValidatedWeekBasket_Consumer_ConsumerId",
                        column: x => x.ConsumerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    AverageQuantity = table.Column<int>(nullable: false),
                    DLC = table.Column<DateTime>(nullable: false),
                    Description = table.Column<string>(nullable: true),
                    FamillyFamillyName = table.Column<string>(nullable: true),
                    LabelsSerialized = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: false),
                    PicturesSerialized = table.Column<string>(nullable: true),
                    Price = table.Column<float>(nullable: false),
                    ProducerId = table.Column<int>(nullable: true),
                    ProductUnit = table.Column<int>(nullable: false),
                    QuantityStep = table.Column<int>(nullable: false),
                    RemainingStock = table.Column<int>(nullable: false),
                    State = table.Column<int>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    WeekStock = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Product", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Product_ProductFamilly_FamillyFamillyName",
                        column: x => x.FamillyFamillyName,
                        principalTable: "ProductFamilly",
                        principalColumn: "FamillyName",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Product_Producer_ProducerId",
                        column: x => x.ProducerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserClaims",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClaimType = table.Column<string>(nullable: true),
                    ClaimValue = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserClaim<string>", x => x.Id);
                    table.ForeignKey(
                        name: "FK_IdentityUserClaim<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserLogins",
                columns: table => new
                {
                    LoginProvider = table.Column<string>(nullable: false),
                    ProviderKey = table.Column<string>(nullable: false),
                    ProviderDisplayName = table.Column<string>(nullable: true),
                    UserId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserLogin<string>", x => new { x.LoginProvider, x.ProviderKey });
                    table.ForeignKey(
                        name: "FK_IdentityUserLogin<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "AspNetUserRoles",
                columns: table => new
                {
                    UserId = table.Column<string>(nullable: false),
                    RoleId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IdentityUserRole<string>", x => new { x.UserId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_IdentityRole_RoleId",
                        column: x => x.RoleId,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IdentityUserRole<string>_ApplicationUser_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
            migrationBuilder.CreateTable(
                name: "BillEntry",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProductId = table.Column<Guid>(nullable: false),
                    Quantity = table.Column<int>(nullable: false),
                    TempWeekBasketId = table.Column<Guid>(nullable: true),
                    ValidatedWeekBasketId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BillEntry", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BillEntry_Product_ProductId",
                        column: x => x.ProductId,
                        principalTable: "Product",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_BillEntry_TempWeekBasket_TempWeekBasketId",
                        column: x => x.TempWeekBasketId,
                        principalTable: "TempWeekBasket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_BillEntry_ValidatedWeekBasket_ValidatedWeekBasketId",
                        column: x => x.ValidatedWeekBasketId,
                        principalTable: "ValidatedWeekBasket",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });
            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName");
            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                table: "AspNetUsers",
                column: "NormalizedEmail");
            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("AspNetRoleClaims");
            migrationBuilder.DropTable("AspNetUserClaims");
            migrationBuilder.DropTable("AspNetUserLogins");
            migrationBuilder.DropTable("AspNetUserRoles");
            migrationBuilder.DropTable("ApplicationConfig");
            migrationBuilder.DropTable("BillEntry");
            migrationBuilder.DropTable("ConsumerBill");
            migrationBuilder.DropTable("News");
            migrationBuilder.DropTable("ProducerBill");
            migrationBuilder.DropTable("AspNetRoles");
            migrationBuilder.DropTable("AspNetUsers");
            migrationBuilder.DropTable("Product");
            migrationBuilder.DropTable("TempWeekBasket");
            migrationBuilder.DropTable("ValidatedWeekBasket");
            migrationBuilder.DropTable("ProductFamilly");
            migrationBuilder.DropTable("User");
            migrationBuilder.DropTable("ProductType");
        }
    }
}
