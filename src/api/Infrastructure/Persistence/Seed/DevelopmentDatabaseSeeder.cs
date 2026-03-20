using FamilyHub.Api.Entities.Calendar;
using FamilyHub.Api.Entities.Catalog;
using FamilyHub.Api.Entities.Orders;
using FamilyHub.Api.Entities.Recipes;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace FamilyHub.Api.Infrastructure.Persistence.Seed;

public static class DevelopmentDatabaseSeeder
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DevelopmentDatabaseSeeder");
        var db = scope.ServiceProvider.GetRequiredService<FamilyHubDbContext>();

        var hasMigrations = db.Database.GetMigrations().Any();

        if (hasMigrations)
        {
            logger.LogInformation("Applying EF Core migrations on startup.");
            try
            {
                await db.Database.MigrateAsync();
                return;
            }
            catch (SqliteException ex) when (ex.Message.Contains("already exists", StringComparison.OrdinalIgnoreCase))
            {
                // Databasen er sandsynligvis oprettet via EnsureCreated og har ikke __EFMigrationsHistory.
                // Genskab databasen fra migrations hvis konfigurationen tillader det.
                var recreate = app.Configuration.GetValue("Database:RecreateIfNoMigrations", true);

                if (!recreate)
                    throw;

                logger.LogWarning(
                    "Database eksisterer allerede uden migrationshistorik. Genskaber SQLite-database fordi Database:RecreateIfNoMigrations=true.");

                await db.Database.EnsureDeletedAsync();
                await db.Database.MigrateAsync();
                return;
            }
        }

        logger.LogInformation("No EF Core migrations found. Using EnsureCreated on startup.");
        await db.Database.EnsureCreatedAsync();

        try
        {
            await db.Database.ExecuteSqlRawAsync("SELECT 1 FROM \"ItemCategories\" LIMIT 1;");
        }
        catch (SqliteException ex) when (ex.Message.Contains("no such table", StringComparison.OrdinalIgnoreCase))
        {
            var recreateIfNoMigrations = app.Configuration.GetValue("Database:RecreateIfNoMigrations", true);

            if (!recreateIfNoMigrations)
                throw;

            logger.LogWarning(
                "Database schema is incompatible and no migrations exist. Recreating SQLite database because Database:RecreateIfNoMigrations=true.");

            await db.Database.EnsureDeletedAsync();
            await db.Database.EnsureCreatedAsync();
        }
    }

    public static async Task MigrateAndSeedDevelopmentDataAsync(this WebApplication app)
    {
        await app.MigrateDatabaseAsync();

        await app.SeedDevelopmentDataIfEmptyAsync();
    }

    public static async Task SeedDevelopmentDataIfEmptyAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DevelopmentDatabaseSeeder");
        var db = scope.ServiceProvider.GetRequiredService<FamilyHubDbContext>();

        var familyMemberCount = await db.FamilyMembers.CountAsync();
        var calendarEventCount = await db.CalendarEvents.CountAsync();
        var itemCategoryCount = await db.ItemCategories.CountAsync();
        var productCount = await db.Products.CountAsync();
        var recipeCategoryCount = await db.RecipeCategories.CountAsync();
        var recipeCount = await db.Recipes.CountAsync();
        var recipeIngredientCount = await db.RecipeIngredients.CountAsync();
        var orderCount = await db.Orders.CountAsync();
        var orderLineCount = await db.OrderLines.CountAsync();

        var totalRows = familyMemberCount
            + calendarEventCount
            + itemCategoryCount
            + productCount
            + recipeCategoryCount
            + recipeCount
            + recipeIngredientCount
            + orderCount
            + orderLineCount;

        if (totalRows > 0)
        {
            logger.LogInformation("Seed skipped because the database already contains data.");
            return;
        }

        logger.LogInformation("Seeding development demo data.");

        await using var transaction = await db.Database.BeginTransactionAsync();

        var familyMembers = new List<FamilyMember>
        {
            new() { Name = "Maja Ramskov", Color = "#22C55E" },
            new() { Name = "Andreas Ramskov", Color = "#0EA5E9" },
            new() { Name = "Liva Ramskov", Color = "#F59E0B" }
        };
        db.FamilyMembers.AddRange(familyMembers);

        var itemCategories = new List<ItemCategory>
        {
            new() { Name = "Mejeri", SortOrder = 10 },
            new() { Name = "Kolonial", SortOrder = 20 },
            new() { Name = "Frugt & Grønt", SortOrder = 30 },
            new() { Name = "Kød & Fisk", SortOrder = 40 },
            new() { Name = "Frost", SortOrder = 50 }
        };
        db.ItemCategories.AddRange(itemCategories);

        var recipeCategories = new List<RecipeCategory>
        {
            new() { Name = "Hverdag", SortOrder = 10 },
            new() { Name = "Weekend", SortOrder = 20 },
            new() { Name = "Bagning", SortOrder = 30 },
            new() { Name = "Hurtig Mad", SortOrder = 40 }
        };
        db.RecipeCategories.AddRange(recipeCategories);

        await db.SaveChangesAsync();

        var mejeri = itemCategories.Single(x => x.Name == "Mejeri");
        var kolonial = itemCategories.Single(x => x.Name == "Kolonial");
        var frugtGroent = itemCategories.Single(x => x.Name == "Frugt & Grønt");
        var koedFisk = itemCategories.Single(x => x.Name == "Kød & Fisk");
        var frost = itemCategories.Single(x => x.Name == "Frost");

        var products = new List<Product>
        {
            new() { Name = "Skyr", ItemCategoryId = mejeri.Id, Unit = "g", SizeLabel = "1 kg", Price = 24.95m, IsManual = true, IsFavorite = true, IsStaple = false, ProteinPer100g = 11.0m },
            new() { Name = "Mælk", ItemCategoryId = mejeri.Id, Unit = "L", SizeLabel = "1 L", Price = 12.50m, IsManual = true, IsFavorite = false, IsStaple = true },
            new() { Name = "Havregryn", ItemCategoryId = kolonial.Id, Unit = "g", SizeLabel = "1 kg", Price = 18.00m, IsManual = true, IsFavorite = true, IsStaple = true, CarbsPer100g = 60.0m },
            new() { Name = "Pasta", ItemCategoryId = kolonial.Id, Unit = "g", SizeLabel = "500 g", Price = 9.50m, IsManual = true, IsFavorite = false, IsStaple = true },
            new() { Name = "Hakkede Tomater", ItemCategoryId = kolonial.Id, Unit = "dåse", SizeLabel = "400 g", Price = 8.00m, IsManual = true, IsFavorite = false, IsStaple = true },
            new() { Name = "Løg", ItemCategoryId = frugtGroent.Id, Unit = "stk", SizeLabel = "1 pose", Price = 14.00m, IsManual = true, IsFavorite = false, IsStaple = true },
            new() { Name = "Gulerødder", ItemCategoryId = frugtGroent.Id, Unit = "g", SizeLabel = "1 kg", Price = 16.00m, IsManual = true, IsFavorite = false, IsStaple = true },
            new() { Name = "Kyllingebryst", ItemCategoryId = koedFisk.Id, Unit = "g", SizeLabel = "600 g", Price = 55.00m, IsManual = true, IsFavorite = true, IsStaple = false, ProteinPer100g = 23.0m },
            new() { Name = "Laks", ItemCategoryId = koedFisk.Id, Unit = "g", SizeLabel = "400 g", Price = 65.00m, IsManual = true, IsFavorite = true, IsStaple = false, ProteinPer100g = 20.0m },
            new() { Name = "Frosne Ærter", ItemCategoryId = frost.Id, Unit = "g", SizeLabel = "500 g", Price = 17.50m, IsManual = true, IsFavorite = false, IsStaple = true }
        };
        db.Products.AddRange(products);

        var nowDate = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var calendarEvents = new List<CalendarEvent>
        {
            new() { Title = "Forældremøde", Description = "Skole i aula", EventDate = nowDate.AddDays(2), StartTime = new TimeOnly(17, 0), EndTime = new TimeOnly(18, 30), FamilyMemberId = familyMembers[0].Id, RecurrenceType = null },
            new() { Title = "Fodboldtræning", Description = "Træning på stadion", EventDate = nowDate.AddDays(1), StartTime = new TimeOnly(16, 30), EndTime = new TimeOnly(18, 0), FamilyMemberId = familyMembers[2].Id, RecurrenceType = "Weekly", RecurrenceDaysJson = "[2,4]" },
            new() { Title = "Indkøbstur", Description = "Ugens storindkøb", EventDate = nowDate.AddDays(3), StartTime = new TimeOnly(19, 0), EndTime = new TimeOnly(20, 0), FamilyMemberId = familyMembers[1].Id, RecurrenceType = "Weekly", RecurrenceDaysJson = "[6]" },
            new() { Title = "Tandlæge", Description = "Kontrol", EventDate = nowDate.AddDays(6), StartTime = new TimeOnly(10, 0), EndTime = new TimeOnly(10, 30), FamilyMemberId = familyMembers[0].Id, RecurrenceType = null },
            new() { Title = "Familieaften", Description = "Brætspil og film", EventDate = nowDate.AddDays(5), StartTime = new TimeOnly(18, 0), EndTime = new TimeOnly(21, 0), FamilyMemberId = null, RecurrenceType = "Monthly" }
        };
        db.CalendarEvents.AddRange(calendarEvents);

        await db.SaveChangesAsync();

        var hverdag = recipeCategories.Single(x => x.Name == "Hverdag");
        var weekend = recipeCategories.Single(x => x.Name == "Weekend");
        var bagning = recipeCategories.Single(x => x.Name == "Bagning");
        var hurtigMad = recipeCategories.Single(x => x.Name == "Hurtig Mad");

        var recipes = new List<Recipe>
        {
            new()
            {
                Title = "Kylling i tomat",
                RecipeCategoryId = hverdag.Id,
                Description = "Nem hverdagsret med kylling og tomat.",
                PrepTimeMinutes = 15,
                WaitTimeMinutes = 20,
                Instructions = "Svits løg, brun kylling, tilsæt tomater og lad simre.",
                IsManual = true,
                IsFavorite = true
            },
            new()
            {
                Title = "Laksepasta",
                RecipeCategoryId = weekend.Id,
                Description = "Cremet pasta med laks.",
                PrepTimeMinutes = 20,
                WaitTimeMinutes = 15,
                Instructions = "Kog pasta, steg laks, vend med sauce.",
                IsManual = true,
                IsFavorite = true
            },
            new()
            {
                Title = "Havregrød",
                RecipeCategoryId = hurtigMad.Id,
                Description = "Hurtig morgenmad.",
                PrepTimeMinutes = 5,
                WaitTimeMinutes = 5,
                Instructions = "Kog havregryn med mælk under omrøring.",
                IsManual = true,
                IsFavorite = false
            },
            new()
            {
                Title = "Gulerodsboller",
                RecipeCategoryId = bagning.Id,
                Description = "Bløde boller med gulerod.",
                PrepTimeMinutes = 25,
                WaitTimeMinutes = 90,
                Instructions = "Rør dej, hæv, form boller og bag.",
                IsManual = true,
                IsFavorite = false
            },
            new()
            {
                Title = "Ærtesuppe",
                RecipeCategoryId = hverdag.Id,
                Description = "Cremet suppe med frosne ærter.",
                PrepTimeMinutes = 10,
                WaitTimeMinutes = 15,
                Instructions = "Kog løg og ærter møre, blend og smag til.",
                IsManual = true,
                IsFavorite = false
            }
        };
        db.Recipes.AddRange(recipes);

        await db.SaveChangesAsync();

        Product? ByName(string name) => products.FirstOrDefault(x => x.Name == name);

        var ingredients = new List<RecipeIngredient>
        {
            new() { RecipeId = recipes[0].Id, ProductId = ByName("Kyllingebryst")?.Id, Name = null, Quantity = 500, Unit = "g", IsStaple = false, SortOrder = 10 },
            new() { RecipeId = recipes[0].Id, ProductId = ByName("Hakkede Tomater")?.Id, Name = null, Quantity = 2, Unit = "dåse", IsStaple = true, SortOrder = 20 },
            new() { RecipeId = recipes[0].Id, ProductId = ByName("Løg")?.Id, Name = null, Quantity = 1, Unit = "stk", IsStaple = true, SortOrder = 30 },

            new() { RecipeId = recipes[1].Id, ProductId = ByName("Laks")?.Id, Name = null, Quantity = 300, Unit = "g", IsStaple = false, SortOrder = 10 },
            new() { RecipeId = recipes[1].Id, ProductId = ByName("Pasta")?.Id, Name = null, Quantity = 350, Unit = "g", IsStaple = true, SortOrder = 20 },
            new() { RecipeId = recipes[1].Id, ProductId = null, Name = "Fløde", Quantity = 2, Unit = "dl", IsStaple = false, SortOrder = 30 },

            new() { RecipeId = recipes[2].Id, ProductId = ByName("Havregryn")?.Id, Name = null, Quantity = 100, Unit = "g", IsStaple = true, SortOrder = 10 },
            new() { RecipeId = recipes[2].Id, ProductId = ByName("Mælk")?.Id, Name = null, Quantity = 3, Unit = "dl", IsStaple = true, SortOrder = 20 },

            new() { RecipeId = recipes[3].Id, ProductId = ByName("Gulerødder")?.Id, Name = null, Quantity = 250, Unit = "g", IsStaple = true, SortOrder = 10 },
            new() { RecipeId = recipes[3].Id, ProductId = null, Name = "Hvedemel", Quantity = 500, Unit = "g", IsStaple = true, SortOrder = 20 },
            new() { RecipeId = recipes[3].Id, ProductId = null, Name = "Gær", Quantity = 25, Unit = "g", IsStaple = false, SortOrder = 30 },

            new() { RecipeId = recipes[4].Id, ProductId = ByName("Frosne Ærter")?.Id, Name = null, Quantity = 400, Unit = "g", IsStaple = false, SortOrder = 10 },
            new() { RecipeId = recipes[4].Id, ProductId = ByName("Løg")?.Id, Name = null, Quantity = 1, Unit = "stk", IsStaple = true, SortOrder = 20 }
        };
        db.RecipeIngredients.AddRange(ingredients);

        var orders = new List<Order>
        {
            new()
            {
                Status = "Completed",
                TotalItems = 9,
                TotalPrice = 136.00m,
                Notes = "Ugens storindkøb til hverdagsmad.",
                Lines =
                [
                    new() { ProductName = "Skyr", CategoryName = "Mejeri", Quantity = 2, Unit = "stk", Price = 24.95m, SizeLabel = "1 kg" },
                    new() { ProductName = "Havregryn", CategoryName = "Kolonial", Quantity = 1, Unit = "stk", Price = 18.00m, SizeLabel = "1 kg" },
                    new() { ProductName = "Løg", CategoryName = "Frugt & Grønt", Quantity = 1, Unit = "pose", Price = 14.00m, SizeLabel = "1 pose" },
                    new() { ProductName = "Gulerødder", CategoryName = "Frugt & Grønt", Quantity = 1, Unit = "stk", Price = 16.00m, SizeLabel = "1 kg" },
                    new() { ProductName = "Kyllingebryst", CategoryName = "Kød & Fisk", Quantity = 1, Unit = "stk", Price = 55.00m, SizeLabel = "600 g" }
                ]
            },
            new()
            {
                Status = "Completed",
                TotalItems = 4,
                TotalPrice = 99.00m,
                PdfData = "JVBERi0xLjQKJUZha2UgRGV2ZWxvcG1lbnQgUERGIGRhdGEgZm9yIEZhbWlseUh1YiBPcmRlcnMu",
                Lines =
                [
                    new() { ProductName = "Laks", CategoryName = "Kød & Fisk", Quantity = 1, Unit = "stk", Price = 65.00m, SizeLabel = "400 g" },
                    new() { ProductName = "Pasta", CategoryName = "Kolonial", Quantity = 2, Unit = "stk", Price = 9.50m, SizeLabel = "500 g" },
                    new() { ProductName = "Hakkede Tomater", CategoryName = "Kolonial", Quantity = 1, Unit = "stk", Price = 8.00m, SizeLabel = "400 g" }
                ]
            },
            new()
            {
                Status = "Created",
                TotalItems = 6,
                TotalPrice = 78.00m,
                Lines =
                [
                    new() { ProductName = "Mælk", CategoryName = "Mejeri", Quantity = 2, Unit = "stk", Price = 12.50m, SizeLabel = "1 L" },
                    new() { ProductName = "Frosne Ærter", CategoryName = "Frost", Quantity = 2, Unit = "stk", Price = 17.50m, SizeLabel = "500 g" },
                    new() { ProductName = "Løg", CategoryName = "Frugt & Grønt", Quantity = 1, Unit = "pose", Price = 14.00m, SizeLabel = "1 pose" },
                    new() { ProductName = "Havregryn", CategoryName = "Kolonial", Quantity = 1, Unit = "stk", Price = 18.00m, SizeLabel = "1 kg" }
                ]
            }
        };
        db.Orders.AddRange(orders);

        await db.SaveChangesAsync();

        await transaction.CommitAsync();
        logger.LogInformation("Development demo data seeded successfully.");
    }
}
