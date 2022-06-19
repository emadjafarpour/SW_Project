namespace BitakShop.Infrastructure.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class bitakshop : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ArticleCategories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 400),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Articles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 600),
                        ShortDescription = c.String(),
                        Description = c.String(),
                        ViewCount = c.Int(nullable: false),
                        Image = c.String(),
                        AddedDate = c.DateTime(),
                        ArticleCategoryId = c.Int(),
                        UserId = c.String(maxLength: 128),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ArticleCategories", t => t.ArticleCategoryId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.ArticleCategoryId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.ArticleComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        Email = c.String(nullable: false, maxLength: 400),
                        Message = c.String(nullable: false, maxLength: 800),
                        AddedDate = c.DateTime(),
                        ParentId = c.Int(),
                        ArticleId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleId, cascadeDelete: true)
                .ForeignKey("dbo.ArticleComments", t => t.ParentId)
                .Index(t => t.ParentId)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.ArticleHeadLines",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 700),
                        Description = c.String(),
                        ArticleId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleId, cascadeDelete: true)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.ArticleTags",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 300),
                        ArticleId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Articles", t => t.ArticleId, cascadeDelete: true)
                .Index(t => t.ArticleId);
            
            CreateTable(
                "dbo.AspNetUsers",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Avatar = c.String(),
                        FirstName = c.String(nullable: false, maxLength: 300),
                        LastName = c.String(nullable: false, maxLength: 300),
                        Information = c.String(),
                        IsDeleted = c.Boolean(nullable: false),
                        Email = c.String(maxLength: 256),
                        EmailConfirmed = c.Boolean(nullable: false),
                        PasswordHash = c.String(),
                        SecurityStamp = c.String(),
                        PhoneNumber = c.String(),
                        PhoneNumberConfirmed = c.Boolean(nullable: false),
                        TwoFactorEnabled = c.Boolean(nullable: false),
                        LockoutEndDateUtc = c.DateTime(),
                        LockoutEnabled = c.Boolean(nullable: false),
                        AccessFailedCount = c.Int(nullable: false),
                        UserName = c.String(nullable: false, maxLength: 256),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.UserName, unique: true, name: "UserNameIndex");
            
            CreateTable(
                "dbo.AspNetUserClaims",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.String(nullable: false, maxLength: 128),
                        ClaimType = c.String(),
                        ClaimValue = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserLogins",
                c => new
                    {
                        LoginProvider = c.String(nullable: false, maxLength: 128),
                        ProviderKey = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId })
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.AspNetUserRoles",
                c => new
                    {
                        UserId = c.String(nullable: false, maxLength: 128),
                        RoleId = c.String(nullable: false, maxLength: 128),
                        Id = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                        Role_Id = c.String(maxLength: 128),
                        User_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => new { t.UserId, t.RoleId })
                .ForeignKey("dbo.AspNetRoles", t => t.Role_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.User_Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId, cascadeDelete: true)
                .Index(t => t.UserId)
                .Index(t => t.RoleId)
                .Index(t => t.Role_Id)
                .Index(t => t.User_Id);
            
            CreateTable(
                "dbo.AspNetRoles",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Name = c.String(nullable: false, maxLength: 256),
                        RoleNameLocal = c.String(maxLength: 300),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.Name, unique: true, name: "RoleNameIndex");
            
            CreateTable(
                "dbo.RolePermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        RoleId = c.String(maxLength: 128),
                        PermissionId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Permissions", t => t.PermissionId, cascadeDelete: true)
                .ForeignKey("dbo.AspNetRoles", t => t.RoleId)
                .Index(t => t.RoleId)
                .Index(t => t.PermissionId);
            
            CreateTable(
                "dbo.Permissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 300),
                        Name = c.String(maxLength: 300),
                        DisplayPriority = c.Int(nullable: false),
                        ControllerName = c.String(maxLength: 300),
                        ActionName = c.String(maxLength: 300),
                        ParentId = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Permissions", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.Brands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Logo = c.String(),
                        Name = c.String(nullable: false),
                        ShortDescription = c.String(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Discounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        GroupIdentifier = c.String(),
                        Title = c.String(),
                        DeadLine = c.DateTime(nullable: false),
                        DiscountType = c.Int(nullable: false),
                        Amount = c.Long(nullable: false),
                        OfferId = c.Int(),
                        ProductId = c.Int(),
                        ProductGroupId = c.Int(),
                        BrandId = c.Int(),
                        DiscountLocationType = c.Int(nullable: false),
                        Image = c.String(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.BrandId)
                .ForeignKey("dbo.Offers", t => t.OfferId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupId)
                .Index(t => t.OfferId)
                .Index(t => t.ProductId)
                .Index(t => t.ProductGroupId)
                .Index(t => t.BrandId);
            
            CreateTable(
                "dbo.Offers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 600),
                        Image = c.String(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ShortTitle = c.String(maxLength: 300),
                        Title = c.String(nullable: false, maxLength: 600),
                        ShortDescription = c.String(maxLength: 2000),
                        Description = c.String(),
                        Image = c.String(),
                        Rate = c.Int(nullable: false),
                        ProductGroupId = c.Int(),
                        BrandId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.BrandId)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupId)
                .Index(t => t.ProductGroupId)
                .Index(t => t.BrandId);
            
            CreateTable(
                "dbo.InvoiceItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Quantity = c.Int(nullable: false),
                        Price = c.Long(nullable: false),
                        TotalPrice = c.Long(nullable: false),
                        ProductId = c.Int(nullable: false),
                        MainFeatureId = c.Int(nullable: false),
                        InvoiceId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                        ProductMainFeature_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.ProductMainFeatures", t => t.ProductMainFeature_Id)
                .Index(t => t.ProductId)
                .Index(t => t.InvoiceId)
                .Index(t => t.ProductMainFeature_Id);
            
            CreateTable(
                "dbo.Invoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiscountAmount = c.Long(nullable: false),
                        TotalPriceBeforeDiscount = c.Long(nullable: false),
                        TotalPrice = c.Long(nullable: false),
                        AddedDate = c.DateTime(nullable: false),
                        InvoiceNumber = c.String(),
                        CustomerId = c.Int(),
                        CustomerName = c.String(maxLength: 500),
                        GeoDivisionId = c.Int(),
                        Address = c.String(maxLength: 500),
                        Phone = c.String(maxLength: 50),
                        PostalCode = c.String(maxLength: 50),
                        Email = c.String(),
                        IsPayed = c.Boolean(nullable: false),
                        DiscountCodeId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .ForeignKey("dbo.DiscountCodes", t => t.DiscountCodeId)
                .ForeignKey("dbo.GeoDivisions", t => t.GeoDivisionId)
                .Index(t => t.CustomerId)
                .Index(t => t.GeoDivisionId)
                .Index(t => t.DiscountCodeId);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NationalCode = c.String(maxLength: 100),
                        Address = c.String(),
                        PostalCode = c.String(maxLength: 100),
                        UserId = c.String(maxLength: 128),
                        GeoDivisionId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GeoDivisions", t => t.GeoDivisionId)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.GeoDivisionId);
            
            CreateTable(
                "dbo.GeoDivisions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(maxLength: 200),
                        LatinTitle = c.String(maxLength: 200),
                        GeoDivisionType = c.Int(nullable: false),
                        IsLeaf = c.Int(),
                        IsArchived = c.Int(),
                        ParentId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.GeoDivisions", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.DiscountCodes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DiscountCodeStr = c.String(),
                        ActivationStartDate = c.DateTime(nullable: false),
                        ActivationEndDate = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        Value = c.Long(nullable: false),
                        CustomerId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Customers", t => t.CustomerId, cascadeDelete: true)
                .Index(t => t.CustomerId);
            
            CreateTable(
                "dbo.EPayments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        Amount = c.Long(nullable: false),
                        Description = c.String(maxLength: 2000),
                        ExtraInfo = c.String(maxLength: 255),
                        RetrievalRefNo = c.String(),
                        SystemTraceNo = c.String(),
                        Token = c.String(),
                        Url = c.String(),
                        PaymentKey = c.String(),
                        Title = c.String(),
                        PaymentStatus = c.Int(nullable: false),
                        PaymentAccountId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Invoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.PaymentAccounts", t => t.PaymentAccountId, cascadeDelete: true)
                .Index(t => t.InvoiceId)
                .Index(t => t.PaymentAccountId);
            
            CreateTable(
                "dbo.EPaymentLogs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(maxLength: 500),
                        LogDate = c.DateTime(nullable: false),
                        LogType = c.String(maxLength: 50),
                        PaymentKey = c.Int(nullable: false),
                        EPaymentId = c.Int(nullable: false),
                        MethodName = c.String(maxLength: 100),
                        Description = c.String(maxLength: 500),
                        Amount = c.Long(nullable: false),
                        RetrievalRefNo = c.String(maxLength: 50),
                        StackTraceNo = c.String(maxLength: 50),
                        Token = c.String(maxLength: 100),
                        Url = c.String(maxLength: 200),
                        ReturnObjectSerialization = c.String(),
                        ReturnUrl = c.String(maxLength: 200),
                        AdditionalData = c.String(maxLength: 500),
                        ResCode = c.String(maxLength: 10),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.EPayments", t => t.EPaymentId, cascadeDelete: true)
                .Index(t => t.EPaymentId);
            
            CreateTable(
                "dbo.PaymentAccounts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(),
                        PIN = c.String(),
                        ComebackUrl = c.String(),
                        PaymentUrl = c.String(),
                        MerchantId = c.Int(nullable: false),
                        TerminalId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductComments",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 300),
                        Email = c.String(nullable: false, maxLength: 400),
                        Message = c.String(nullable: false, maxLength: 800),
                        AddedDate = c.DateTime(),
                        Rate = c.Int(),
                        ParentId = c.Int(),
                        ProductId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductComments", t => t.ParentId)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .Index(t => t.ParentId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ProductFeatureValues",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(maxLength: 600),
                        OtherInfo = c.String(maxLength: 600),
                        FeatureId = c.Int(),
                        ProductId = c.Int(),
                        SubFeatureId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Features", t => t.FeatureId)
                .ForeignKey("dbo.SubFeatures", t => t.SubFeatureId)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.FeatureId)
                .Index(t => t.ProductId)
                .Index(t => t.SubFeatureId);
            
            CreateTable(
                "dbo.Features",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 600),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductGroupFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductGroupId = c.Int(nullable: false),
                        FeatureId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Features", t => t.FeatureId, cascadeDelete: true)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupId, cascadeDelete: true)
                .Index(t => t.ProductGroupId)
                .Index(t => t.FeatureId);
            
            CreateTable(
                "dbo.ProductGroups",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 600),
                        Image = c.String(),
                        ParentId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ProductGroups", t => t.ParentId)
                .Index(t => t.ParentId);
            
            CreateTable(
                "dbo.ProductGroupBrands",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductGroupId = c.Int(nullable: false),
                        BrandId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Brands", t => t.BrandId, cascadeDelete: true)
                .ForeignKey("dbo.ProductGroups", t => t.ProductGroupId, cascadeDelete: true)
                .Index(t => t.ProductGroupId)
                .Index(t => t.BrandId);
            
            CreateTable(
                "dbo.ProductMainFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(maxLength: 600),
                        OtherInfo = c.String(maxLength: 600),
                        FeatureId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        SubFeatureId = c.Int(),
                        Quantity = c.Int(nullable: false),
                        Price = c.Long(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Features", t => t.FeatureId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId, cascadeDelete: true)
                .ForeignKey("dbo.SubFeatures", t => t.SubFeatureId)
                .Index(t => t.FeatureId)
                .Index(t => t.ProductId)
                .Index(t => t.SubFeatureId);
            
            CreateTable(
                "dbo.SubFeatures",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Value = c.String(nullable: false, maxLength: 600),
                        OtherInfo = c.String(maxLength: 600),
                        Image = c.String(),
                        FeatureId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Features", t => t.FeatureId)
                .Index(t => t.FeatureId);
            
            CreateTable(
                "dbo.ProductGalleries",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.String(),
                        Title = c.String(nullable: false),
                        ProductId = c.Int(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.ContactForms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 600),
                        Phone = c.String(nullable: false, maxLength: 600),
                        Email = c.String(nullable: false, maxLength: 600),
                        Message = c.String(nullable: false),
                        IsViewed = c.Boolean(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Faqs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Question = c.String(nullable: false),
                        Answer = c.String(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Logs",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(maxLength: 200),
                        TableName = c.String(maxLength: 100),
                        EntityId = c.Int(nullable: false),
                        Action = c.String(maxLength: 100),
                        ActionDate = c.DateTime(nullable: false),
                        OldValue = c.String(),
                        NewValue = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Partners",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Image = c.String(),
                        Title = c.String(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.StaticContentDetails",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 600),
                        ShortDescription = c.String(),
                        Description = c.String(),
                        Image = c.String(),
                        Link = c.String(),
                        StaticContentTypeId = c.Int(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.StaticContentTypes", t => t.StaticContentTypeId, cascadeDelete: true)
                .Index(t => t.StaticContentTypeId);
            
            CreateTable(
                "dbo.StaticContentTypes",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 600),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.TeamMembers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        JobTitle = c.String(nullable: false),
                        Image = c.String(),
                        InstagramPage = c.String(),
                        TelegramChannel = c.String(),
                        YoutubePage = c.String(),
                        TwitterPage = c.String(),
                        AddedDate = c.DateTime(),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Testimonials",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false),
                        Image = c.String(),
                        Description = c.String(nullable: false),
                        InsertUser = c.String(),
                        InsertDate = c.DateTime(),
                        UpdateUser = c.String(),
                        UpdateDate = c.DateTime(),
                        IsDeleted = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.StaticContentDetails", "StaticContentTypeId", "dbo.StaticContentTypes");
            DropForeignKey("dbo.AspNetUserRoles", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.ProductGalleries", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductFeatureValues", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductMainFeatures", "SubFeatureId", "dbo.SubFeatures");
            DropForeignKey("dbo.ProductFeatureValues", "SubFeatureId", "dbo.SubFeatures");
            DropForeignKey("dbo.SubFeatures", "FeatureId", "dbo.Features");
            DropForeignKey("dbo.ProductMainFeatures", "ProductId", "dbo.Products");
            DropForeignKey("dbo.InvoiceItems", "ProductMainFeature_Id", "dbo.ProductMainFeatures");
            DropForeignKey("dbo.ProductMainFeatures", "FeatureId", "dbo.Features");
            DropForeignKey("dbo.Products", "ProductGroupId", "dbo.ProductGroups");
            DropForeignKey("dbo.ProductGroupFeatures", "ProductGroupId", "dbo.ProductGroups");
            DropForeignKey("dbo.ProductGroupBrands", "ProductGroupId", "dbo.ProductGroups");
            DropForeignKey("dbo.ProductGroupBrands", "BrandId", "dbo.Brands");
            DropForeignKey("dbo.Discounts", "ProductGroupId", "dbo.ProductGroups");
            DropForeignKey("dbo.ProductGroups", "ParentId", "dbo.ProductGroups");
            DropForeignKey("dbo.ProductGroupFeatures", "FeatureId", "dbo.Features");
            DropForeignKey("dbo.ProductFeatureValues", "FeatureId", "dbo.Features");
            DropForeignKey("dbo.ProductComments", "ProductId", "dbo.Products");
            DropForeignKey("dbo.ProductComments", "ParentId", "dbo.ProductComments");
            DropForeignKey("dbo.InvoiceItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.InvoiceItems", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.Invoices", "GeoDivisionId", "dbo.GeoDivisions");
            DropForeignKey("dbo.EPayments", "PaymentAccountId", "dbo.PaymentAccounts");
            DropForeignKey("dbo.EPayments", "InvoiceId", "dbo.Invoices");
            DropForeignKey("dbo.EPaymentLogs", "EPaymentId", "dbo.EPayments");
            DropForeignKey("dbo.Invoices", "DiscountCodeId", "dbo.DiscountCodes");
            DropForeignKey("dbo.DiscountCodes", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Customers", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Invoices", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.Customers", "GeoDivisionId", "dbo.GeoDivisions");
            DropForeignKey("dbo.GeoDivisions", "ParentId", "dbo.GeoDivisions");
            DropForeignKey("dbo.Discounts", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "BrandId", "dbo.Brands");
            DropForeignKey("dbo.Discounts", "OfferId", "dbo.Offers");
            DropForeignKey("dbo.Discounts", "BrandId", "dbo.Brands");
            DropForeignKey("dbo.AspNetUserRoles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "User_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserRoles", "Role_Id", "dbo.AspNetRoles");
            DropForeignKey("dbo.RolePermissions", "RoleId", "dbo.AspNetRoles");
            DropForeignKey("dbo.RolePermissions", "PermissionId", "dbo.Permissions");
            DropForeignKey("dbo.Permissions", "ParentId", "dbo.Permissions");
            DropForeignKey("dbo.AspNetUserLogins", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserClaims", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.Articles", "UserId", "dbo.AspNetUsers");
            DropForeignKey("dbo.ArticleTags", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.ArticleHeadLines", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.ArticleComments", "ParentId", "dbo.ArticleComments");
            DropForeignKey("dbo.ArticleComments", "ArticleId", "dbo.Articles");
            DropForeignKey("dbo.Articles", "ArticleCategoryId", "dbo.ArticleCategories");
            DropIndex("dbo.StaticContentDetails", new[] { "StaticContentTypeId" });
            DropIndex("dbo.ProductGalleries", new[] { "ProductId" });
            DropIndex("dbo.SubFeatures", new[] { "FeatureId" });
            DropIndex("dbo.ProductMainFeatures", new[] { "SubFeatureId" });
            DropIndex("dbo.ProductMainFeatures", new[] { "ProductId" });
            DropIndex("dbo.ProductMainFeatures", new[] { "FeatureId" });
            DropIndex("dbo.ProductGroupBrands", new[] { "BrandId" });
            DropIndex("dbo.ProductGroupBrands", new[] { "ProductGroupId" });
            DropIndex("dbo.ProductGroups", new[] { "ParentId" });
            DropIndex("dbo.ProductGroupFeatures", new[] { "FeatureId" });
            DropIndex("dbo.ProductGroupFeatures", new[] { "ProductGroupId" });
            DropIndex("dbo.ProductFeatureValues", new[] { "SubFeatureId" });
            DropIndex("dbo.ProductFeatureValues", new[] { "ProductId" });
            DropIndex("dbo.ProductFeatureValues", new[] { "FeatureId" });
            DropIndex("dbo.ProductComments", new[] { "ProductId" });
            DropIndex("dbo.ProductComments", new[] { "ParentId" });
            DropIndex("dbo.EPaymentLogs", new[] { "EPaymentId" });
            DropIndex("dbo.EPayments", new[] { "PaymentAccountId" });
            DropIndex("dbo.EPayments", new[] { "InvoiceId" });
            DropIndex("dbo.DiscountCodes", new[] { "CustomerId" });
            DropIndex("dbo.GeoDivisions", new[] { "ParentId" });
            DropIndex("dbo.Customers", new[] { "GeoDivisionId" });
            DropIndex("dbo.Customers", new[] { "UserId" });
            DropIndex("dbo.Invoices", new[] { "DiscountCodeId" });
            DropIndex("dbo.Invoices", new[] { "GeoDivisionId" });
            DropIndex("dbo.Invoices", new[] { "CustomerId" });
            DropIndex("dbo.InvoiceItems", new[] { "ProductMainFeature_Id" });
            DropIndex("dbo.InvoiceItems", new[] { "InvoiceId" });
            DropIndex("dbo.InvoiceItems", new[] { "ProductId" });
            DropIndex("dbo.Products", new[] { "BrandId" });
            DropIndex("dbo.Products", new[] { "ProductGroupId" });
            DropIndex("dbo.Discounts", new[] { "BrandId" });
            DropIndex("dbo.Discounts", new[] { "ProductGroupId" });
            DropIndex("dbo.Discounts", new[] { "ProductId" });
            DropIndex("dbo.Discounts", new[] { "OfferId" });
            DropIndex("dbo.Permissions", new[] { "ParentId" });
            DropIndex("dbo.RolePermissions", new[] { "PermissionId" });
            DropIndex("dbo.RolePermissions", new[] { "RoleId" });
            DropIndex("dbo.AspNetRoles", "RoleNameIndex");
            DropIndex("dbo.AspNetUserRoles", new[] { "User_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "Role_Id" });
            DropIndex("dbo.AspNetUserRoles", new[] { "RoleId" });
            DropIndex("dbo.AspNetUserRoles", new[] { "UserId" });
            DropIndex("dbo.AspNetUserLogins", new[] { "UserId" });
            DropIndex("dbo.AspNetUserClaims", new[] { "UserId" });
            DropIndex("dbo.AspNetUsers", "UserNameIndex");
            DropIndex("dbo.ArticleTags", new[] { "ArticleId" });
            DropIndex("dbo.ArticleHeadLines", new[] { "ArticleId" });
            DropIndex("dbo.ArticleComments", new[] { "ArticleId" });
            DropIndex("dbo.ArticleComments", new[] { "ParentId" });
            DropIndex("dbo.Articles", new[] { "UserId" });
            DropIndex("dbo.Articles", new[] { "ArticleCategoryId" });
            DropTable("dbo.Testimonials");
            DropTable("dbo.TeamMembers");
            DropTable("dbo.StaticContentTypes");
            DropTable("dbo.StaticContentDetails");
            DropTable("dbo.Partners");
            DropTable("dbo.Logs");
            DropTable("dbo.Faqs");
            DropTable("dbo.ContactForms");
            DropTable("dbo.ProductGalleries");
            DropTable("dbo.SubFeatures");
            DropTable("dbo.ProductMainFeatures");
            DropTable("dbo.ProductGroupBrands");
            DropTable("dbo.ProductGroups");
            DropTable("dbo.ProductGroupFeatures");
            DropTable("dbo.Features");
            DropTable("dbo.ProductFeatureValues");
            DropTable("dbo.ProductComments");
            DropTable("dbo.PaymentAccounts");
            DropTable("dbo.EPaymentLogs");
            DropTable("dbo.EPayments");
            DropTable("dbo.DiscountCodes");
            DropTable("dbo.GeoDivisions");
            DropTable("dbo.Customers");
            DropTable("dbo.Invoices");
            DropTable("dbo.InvoiceItems");
            DropTable("dbo.Products");
            DropTable("dbo.Offers");
            DropTable("dbo.Discounts");
            DropTable("dbo.Brands");
            DropTable("dbo.Permissions");
            DropTable("dbo.RolePermissions");
            DropTable("dbo.AspNetRoles");
            DropTable("dbo.AspNetUserRoles");
            DropTable("dbo.AspNetUserLogins");
            DropTable("dbo.AspNetUserClaims");
            DropTable("dbo.AspNetUsers");
            DropTable("dbo.ArticleTags");
            DropTable("dbo.ArticleHeadLines");
            DropTable("dbo.ArticleComments");
            DropTable("dbo.Articles");
            DropTable("dbo.ArticleCategories");
        }
    }
}
