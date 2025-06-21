
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using PassionStore.Core.Entities;
using PassionStore.Core.Models;
using PassionStore.Core.Models.Auth;
using PassionStore.Infrastructure.Data;

namespace PassionStore.Api.SeedData
{
    public class DbInitializer
    {
        public static async Task InitDb(WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;

            var userManager = services.GetRequiredService<UserManager<AppUser>>()
                ?? throw new InvalidOperationException("Failed to retrieve user manager");
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole<Guid>>>()
                ?? throw new InvalidOperationException("Failed to retrieve role manager");

            await SeedUsersAndRoles(userManager, roleManager);
            await services.GetRequiredService<AppDbContext>().SaveChangesAsync(); // Ensure users are saved

            var dbContext = services.GetRequiredService<AppDbContext>()
                ?? throw new InvalidOperationException("Failed to retrieve app db context");

            await SeedAddresses(dbContext);
            await dbContext.SaveChangesAsync(); // Ensure addresses are saved
            await SeedUserProfiles(dbContext, userManager);
            await SeedOrders(dbContext);
            await SeedCategory(dbContext);
            await SeedSubCategories(dbContext);
            await SeedBrands(dbContext);
            await SeedColors(dbContext);
            await SeedSizes(dbContext);
            await SeedProducts(dbContext);
            await SeedProductVariants(dbContext);
            await SeedOrderItems(dbContext);
            await SeedCartItems(dbContext);
            await SeedVerifyCodes(dbContext, userManager);
            await SeedReports(dbContext, userManager);
            await SeedHistories(dbContext, userManager);
            await SeedNotifications(dbContext, userManager);
            await SeedChats(dbContext, userManager);
            await SeedMessages(dbContext);
            await SeedRatings(dbContext, userManager);
        }

        private static async Task SeedUsersAndRoles(UserManager<AppUser> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            if (!roleManager.Roles.Any())
            {
                foreach (var role in Enum.GetValues<UserRole>())
                {
                    if (!await roleManager.RoleExistsAsync(role.ToString()))
                    {
                        await roleManager.CreateAsync(new IdentityRole<Guid>(role.ToString()));
                    }
                }
            }

            if (!userManager.Users.Any(x => !string.IsNullOrEmpty(x.Email)))
            {
                var password = "Luantang@123!";
                var users = new List<(string UserName, string Email, UserRole Role)>
                {
                    ("luantang", "luantang.work@gmail.com", UserRole.User),
                    ("admin", "admin@gmail.com", UserRole.Admin),
                    ("johndoe", "john.doe@gmail.com", UserRole.User),
                    ("janedoe", "jane.doe@gmail.com", UserRole.User),
                    ("marysmith", "mary.smith@gmail.com", UserRole.User)
                };

                foreach (var userData in users)
                {
                    var newUser = new AppUser
                    {
                        UserName = userData.UserName,
                        Email = userData.Email,
                        EmailConfirmed = true
                    };
                    var result = await userManager.CreateAsync(newUser, password);
                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(newUser, userData.Role.ToString());
                    }
                }
            }
        }

        private static async Task SeedAddresses(AppDbContext context)
        {
            if (!context.Addresses.Any())
            {
                var addresses = new List<Address>
                {
                    new Address { Street = "123 Main St", City = "Hanoi", State = "Ha Noi", PostalCode = "10000", Country = "Vietnam" },
                    new Address { Street = "456 Oak Ave", City = "Ho Chi Minh", State = "Ho Chi Minh", PostalCode = "70000", Country = "Vietnam" }
                };

                await context.Addresses.AddRangeAsync(addresses);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedUserProfiles(AppDbContext context, UserManager<AppUser> userManager)
        {
            if (!context.UserProfiles.Any())
            {
                var luanUser = await userManager.FindByEmailAsync("luantang.work@gmail.com");
                var adminUser = await userManager.FindByEmailAsync("admin@gmail.com");
                var addresses = await context.Addresses.ToListAsync();

                if (luanUser == null || adminUser == null || !addresses.Any())
                {
                    throw new InvalidOperationException("Required users or addresses not found for seeding user profiles.");
                }

                var userProfiles = new List<UserProfile>
                {
                    new UserProfile { FullName = "Luan Tang", PhoneNumber = "0901234567", UserId = luanUser.Id, AddressId = addresses[0].Id },
                    new UserProfile { FullName = "Admin User", PhoneNumber = "0907654321", UserId = adminUser.Id, AddressId = addresses[1].Id }
                };

                await context.UserProfiles.AddRangeAsync(userProfiles);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedOrders(AppDbContext context)
        {
            if (!context.Orders.Any())
            {
                var userProfiles = await context.UserProfiles.ToListAsync();
                var addresses = await context.Addresses.ToListAsync();

                if (!userProfiles.Any() || !addresses.Any())
                {
                    throw new InvalidOperationException("Required user profiles or addresses not found for seeding orders.");
                }

                var orders = new List<Order>
                {
                    new Order
                    {
                        TotalAmount = 300,
                        Status = "Pending",
                        OrderDate = DateTime.UtcNow,
                        UserProfileId = userProfiles[0].Id,
                        ShippingAddressId = addresses[0].Id
                    },
                    new Order
                    {
                        TotalAmount = 200,
                        Status = "Completed",
                        OrderDate = DateTime.UtcNow.AddDays(-1),
                        UserProfileId = userProfiles[1].Id,
                        ShippingAddressId = addresses[1].Id
                    }
                };

                await context.Orders.AddRangeAsync(orders);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedCategory(AppDbContext context)
        {
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Category 1", Description = "Description 1" },
                    new Category { Name = "Category 2", Description = "Description 2" },
                    new Category { Name = "Category 3", Description = "Description 3" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedSubCategories(AppDbContext context)
        {
            var parentCategories = await context.Categories
                .Where(c => c.Name == "Category 1" || c.Name == "Category 2")
                .ToListAsync();

            if (!context.Categories.Any(c => c.Level > 0))
            {
                var subCategories = new List<Category>
                {
                    new Category
                    {
                        Name = "SubCategory 1.1",
                        Description = "Sub of Category 1",
                        Level = 1,
                        ParentCategoryId = parentCategories.First(c => c.Name == "Category 1").Id
                    },
                    new Category
                    {
                        Name = "SubCategory 1.2",
                        Description = "Another sub of Category 1",
                        Level = 1,
                        ParentCategoryId = parentCategories.First(c => c.Name == "Category 1").Id
                    },
                    new Category
                    {
                        Name = "SubCategory 2.1",
                        Description = "Sub of Category 2",
                        Level = 1,
                        ParentCategoryId = parentCategories.First(c => c.Name == "Category 2").Id
                    }
                };

                await context.Categories.AddRangeAsync(subCategories);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedBrands(AppDbContext context)
        {
            if (!context.Brands.Any())
            {
                var brands = new List<Brand>
                {
                    new Brand { Name = "Brand A", Description = "Description for Brand A" },
                    new Brand { Name = "Brand B", Description = "Description for Brand B" }
                };

                await context.Brands.AddRangeAsync(brands);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedColors(AppDbContext context)
        {
            if (!await context.Colors.AnyAsync())
            {
                var colors = new List<Color>
                {
                    new Color { Name = "Red", HexCode = "#FF0000" },
                    new Color { Name = "Blue", HexCode = "#0000FF" }
                };

                await context.Colors.AddRangeAsync(colors);
                await context.SaveChangesAsync();
            }
            if (await context.Colors.FirstOrDefaultAsync(x => x.Id == Guid.Parse("4c3d578a-1a50-4c18-be4e-2bf05f2ca456")) == null)
            {
                var noneColor = new Color
                {
                    Id = Guid.Parse("4c3d578a-1a50-4c18-be4e-2bf05f2ca456"),
                    Name = "None",
                    HexCode = "None"
                };
                await context.Colors.AddAsync(noneColor);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedSizes(AppDbContext context)
        {
            if (!await context.Sizes.AnyAsync())
            {
                var sizes = new List<Size>
                {
                    new Size { Name = "S" },
                    new Size { Name = "M" }
                };

                await context.Sizes.AddRangeAsync(sizes);
                await context.SaveChangesAsync();
            }
            if (await context.Sizes.FirstOrDefaultAsync(x => x.Id == Guid.Parse("c34710ac-d87f-479c-9528-4e21ae9331d5")) == null)
            {
                var noneSize = new Size
                {
                    Id = Guid.Parse("c34710ac-d87f-479c-9528-4e21ae9331d5"),
                    Name = "None"
                };
                await context.Sizes.AddAsync(noneSize);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProducts(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                var existingCategories = await context.Categories
                    .Where(c => c.Name == "Category 1" || c.Name == "Category 2" || c.Name == "Category 3")
                    .ToListAsync();
                var brands = await context.Brands.ToListAsync();

                var product1 = new Product
                {
                    Name = "Product 1",
                    Description = "Description for product 1",
                    InStock = true,
                    BrandId = brands[0].Id,
                    Categories = new List<Category> { existingCategories[0], existingCategories[1] },
                    ProductImages = new List<ProductImage>
                    {
                        new ProductImage
                        {
                            ImageUrl = "https://down-vn.img.susercontent.com/file/850dbf89d966ffcd43feb9ea148f6634@resize_w900_nl.webp",
                            PublicId = "sample1",
                        },
                        new ProductImage
                        {
                            ImageUrl = "https://down-vn.img.susercontent.com/file/3251977c3cd7ad7dc3b076c88610df27.webp",
                            PublicId = "sample2",
                        }
                    }
                };

                var product2 = new Product
                {
                    Name = "Product 2",
                    Description = "Description for product 2",
                    InStock = true,
                    BrandId = brands[1].Id,
                    Categories = new List<Category> { existingCategories[2] },
                    ProductImages = new List<ProductImage>
                    {
                        new ProductImage
                        {
                            ImageUrl = "https://down-vn.img.susercontent.com/file/be401154665999f78c2b8a177300284d.webp",
                            PublicId = "sample3",
                        },
                        new ProductImage
                        {
                            ImageUrl = "https://down-vn.img.susercontent.com/file/55c1f54f43b0416b5530bb230b55976c.webp",
                            PublicId = "sample4",
                        },
                        new ProductImage
                        {
                            ImageUrl = "https://down-vn.img.susercontent.com/file/f78263b4c5fbd8a97b7f44db7b24cd06.webp",
                            PublicId = "sample5",
                        }
                    }
                };

                await context.Products.AddRangeAsync(product1, product2);
                await context.SaveChangesAsync();
            }
        }

        private static async Task SeedProductVariants(AppDbContext context)
        {
            if (!context.ProductVariants.Any())
            {
                var products = await context.Products.ToListAsync();
                var colors = await context.Colors.ToListAsync();
                var sizes = await context.Sizes.ToListAsync();

                var productVariants = new List<ProductVariant>
                {
                    new ProductVariant
                    {
                        Price = 100,
                        StockQuantity = 50,
                        ProductId = products[0].Id,
                        ColorId = colors[0].Id,
                        SizeId = sizes[0].Id
                    },
                    new ProductVariant
                    {
                        Price = 20,
                        StockQuantity = 100,
                        ProductId = products[1].Id,
                        ColorId = colors[1].Id,
                        SizeId = sizes[1].Id
                    }
                };

                await context.ProductVariants.AddRangeAsync(productVariants);
                await context.SaveChangesAsync();
            }
        }

      
        private static async Task SeedOrderItems(AppDbContext context)
        {
            if (!context.OrderItems.Any())
            {
                var orders = await context.Orders.ToListAsync();
                var productVariants = await context.ProductVariants.ToListAsync();

                if (orders.Any() && productVariants.Any())
                {
                    var orderItems = new List<OrderItem>
                    {
                        new OrderItem
                        {
                            Quantity = 2,
                            Price = 100,
                            ProductVariantId = productVariants[0].Id,
                            OrderId = orders[0].Id
                        },
                        new OrderItem
                        {
                            Quantity = 1,
                            Price = 200,
                            ProductVariantId = productVariants[1].Id,
                            OrderId = orders[0].Id
                        },
                        new OrderItem
                        {
                            Quantity = 1,
                            Price = 200,
                            ProductVariantId = productVariants[1].Id,
                            OrderId = orders[1].Id
                        }
                    };

                    await context.OrderItems.AddRangeAsync(orderItems);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedCartItems(AppDbContext context)
        {
            if (!context.CartItems.Any())
            {
                var carts = await context.Carts.ToListAsync();
                var productVariants = await context.ProductVariants.ToListAsync();

                if (carts.Any() && productVariants.Any())
                {
                    var cartItems = new List<CartItem>
                    {
                        new CartItem
                        {
                            Quantity = 1,
                            Price = 100,
                            ProductVariantId = productVariants[0].Id,
                            CartId = carts[0].Id
                        },
                        new CartItem
                        {
                            Quantity = 2,
                            Price = 200,
                            ProductVariantId = productVariants[1].Id,
                            CartId = carts[0].Id
                        }
                    };

                    await context.CartItems.AddRangeAsync(cartItems);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedVerifyCodes(AppDbContext context, UserManager<AppUser> userManager)
        {
            if (!context.VerifyCodes.Any())
            {
                var user = await userManager.FindByEmailAsync("luantang.work@gmail.com");

                if (user != null)
                {
                    var verifyCode = new VerifyCode
                    {
                        Code = "123456",
                        IsVerified = false,
                        ExpiryTime = DateTime.UtcNow.AddHours(1),
                        UserId = user.Id
                    };

                    await context.VerifyCodes.AddAsync(verifyCode);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedReports(AppDbContext context, UserManager<AppUser> userManager)
        {
            if (!context.Reports.Any())
            {
                var user = await userManager.FindByEmailAsync("luantang.work@gmail.com");

                if (user != null)
                {
                    var report = new Report
                    {
                        Content = "Issue with product delivery",
                        ObjectId = (await context.Products.FirstAsync()).Id,
                        ObjectType = "Product",
                        Processed = false,
                        SenderId = user.Id
                    };

                    await context.Reports.AddAsync(report);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedHistories(AppDbContext context, UserManager<AppUser> userManager)
        {
            if (!context.Histories.Any())
            {
                var user = await userManager.FindByEmailAsync("luantang.work@gmail.com");

                if (user != null)
                {
                    var history = new History
                    {
                        ObjectId = (await context.Products.FirstAsync()).Id,
                        ObjectType = "Product",
                        Action = "Viewed",
                        UserId = user.Id
                    };

                    await context.Histories.AddAsync(history);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedNotifications(AppDbContext context, UserManager<AppUser> userManager)
        {
            if (!context.Notifications.Any())
            {
                var user = await userManager.FindByEmailAsync("luantang.work@gmail.com");

                if (user != null)
                {
                    var notification = new Notification
                    {
                        Content = "Your order has been shipped",
                        ObjectId = (await context.Orders.FirstAsync()).Id,
                        ObjectType = "Order",
                        IsRead = false,
                        UserId = user.Id
                    };

                    await context.Notifications.AddAsync(notification);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedChats(AppDbContext context, UserManager<AppUser> userManager)
        {
            if (!context.Chats.Any())
            {
                var user = await userManager.FindByEmailAsync("luantang.work@gmail.com");

                if (user != null)
                {
                    var chat = new Chat
                    {
                        Topic = "Support Request",
                        UserId = user.Id
                    };

                    await context.Chats.AddAsync(chat);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedMessages(AppDbContext context)
        {
            if (!context.Messages.Any())
            {
                var chats = await context.Chats.ToListAsync();

                if (chats.Any())
                {
                    var message = new Message
                    {
                        Content = "Hello, how can I assist you?",
                        IsUserMessage = false,
                        ChatId = chats[0].Id
                    };

                    await context.Messages.AddAsync(message);
                    await context.SaveChangesAsync();
                }
            }
        }

        private static async Task SeedRatings(AppDbContext context, UserManager<AppUser> userManager)
        {
            if (!context.Ratings.Any())
            {
                var user = await userManager.FindByEmailAsync("luantang.work@gmail.com");
                var products = await context.Products
                    .Where(p => p.Name == "Product 1" || p.Name == "Product 2")
                    .ToListAsync();

                if (user != null && products.Any())
                {
                    var ratings = new List<Rating>
                    {
                        new Rating
                        {
                            Value = 5,
                            Comment = "Great product, highly recommend!",
                            ProductId = products[0].Id,
                            UserId = user.Id
                        },
                        new Rating
                        {
                            Value = 4,
                            Comment = "Good quality, but shipping was slow.",
                            ProductId = products[1].Id,
                            UserId = user.Id
                        }
                    };

                    await context.Ratings.AddRangeAsync(ratings);
                    await context.SaveChangesAsync();
                }
            }
        }
    }
}