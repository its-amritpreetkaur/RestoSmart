using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RestoSmart.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using static RestoSmart.Controllers.HomeController;

namespace RestoSmart.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Welcome()
        {
            return View();
        }
        
        public IActionResult Index(int? tableId)
        {
            if (tableId.HasValue)
            {
                HttpContext.Session.SetInt32("CustomerTable", tableId.Value);
            }

            ViewBag.CurrentTable = HttpContext.Session.GetInt32("CustomerTable") ?? 1;

            var menu = _context.MenuItems.FromSqlRaw("SELECT * FROM MENU_ITEMS").ToList();
            return View(menu);
        }

        public IActionResult Dashboard()
        {
            var headers = _context.SalesHeaders
                         .Where(h => h.Status == "Pending")
                         .OrderByDescending(h => h.BillId)
                         .ToList();

            var orders = new List<OrderDashboardViewModel>();

            foreach (var h in headers)
            {
                var dishes = _context.SalesDetails
                    .Where(d => d.SH_BillID == h.BillId)
                    .Join(_context.MenuItems,
                          d => d.DishId,
                          m => m.DishId,
                          (d, m) => new OrderItemViewModel
                          {
                              DishName = m.Name,
                              Qty = d.QtySold
                          }).ToList();

                orders.Add(new OrderDashboardViewModel
                {
                    SH_BillID = h.BillId,
                    Date = h.Date,
                    TableId = h.TableId ?? 0,
                    TotalAmount = h.TotalAmount,
                    Items = dishes
                });
            }

            return View(orders);
        }

       
        public IActionResult Kitchen()
        {
            var pendingHeaders = _context.SalesHeaders
                .Where(h => h.Status == "Pending")
                .OrderBy(h => h.BillId)
                .ToList();

            var orders = new List<OrderDashboardViewModel>();

            foreach (var h in pendingHeaders)
            {
                var dishes = _context.SalesDetails
                    .Where(d => d.SH_BillID == h.BillId)
                    .Join(_context.MenuItems,
                          d => d.DishId,
                          m => m.DishId,
                          (d, m) => new OrderItemViewModel
                          {
                              DishName = m.Name,
                              Qty = d.QtySold
                          }).ToList();

                orders.Add(new OrderDashboardViewModel
                {
                    SH_BillID = h.BillId,
                    Date = h.Date,
                    TableId = h.TableId ?? 0,
                    Items = dishes
                });
            }

            return View(orders);
        }
        public IActionResult AddDish()
        {
            ViewBag.Inventory = _context.RawMaterials.FromSqlRaw("SELECT * FROM RAW_MATERIALS").ToList();
            return View();
        }
        [HttpPost]
        public IActionResult AddDish(MenuItem dish, int[] ingredients, decimal[] qtys)
        {
            try
            {
                int nextId = (_context.MenuItems.Any() ? _context.MenuItems.Max(m => m.DishId) : 0) + 1;
                dish.DishId = nextId;
                _context.MenuItems.Add(dish);
                _context.SaveChanges();

                if (ingredients != null)
                {
                    for (int i = 0; i < ingredients.Length; i++)
                    {
                        if (ingredients[i] > 0) 
                        {
                            int rId = (_context.Recipes.Any() ? _context.Recipes.Max(r => r.RecipeId) : 0) + 1;
                            _context.Database.ExecuteSqlRaw(
                                "INSERT INTO RECIPES (R_RECIPEID, MI_DISHID, I_INGREDIENTID, R_QTYREQUIRED) VALUES ({0}, {1}, {2}, {3})",
                                rId, nextId, ingredients[i], qtys[i]
                            );
                        }
                    }
                }
                return RedirectToAction("ManageMenu");
            }
            catch (Exception ex)
            {
                ViewBag.Inventory = _context.RawMaterials.ToList(); 
                return View(dish);
            }
        }

        [HttpPost]
        public IActionResult QuickAddMaterial([FromBody] RawMaterial mat)
        {
            try
            {
                int nextId = (_context.RawMaterials.Any() ? _context.RawMaterials.Max(r => r.Id) : 0) + 1;

                _context.Database.ExecuteSqlRaw(
                    "INSERT INTO RAW_MATERIALS (RM_ID, RM_NAME, RM_UNIT, RM_CURRENTSTOCK, RM_MINREORDER) VALUES ({0}, {1}, {2}, {3}, {4})",
                    nextId, mat.Name, mat.Unit, mat.CurrentStock, 1.0
                );

                return Json(new { success = true, id = nextId });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            
            var user = _context.Users
                .FromSqlRaw("SELECT * FROM USERS WHERE U_USERNAME = {0} AND U_PASSWORD = {1}", username, password)
                .AsEnumerable()
                .FirstOrDefault();

            if (user == null)
            {
                ViewBag.Error = "Invalid username or password!";
                return View();
            }

            HttpContext.Session.SetString("UserRole", user.Role);

            if (user.Role.Equals("Admin", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Dashboard");
            }
            else if (user.Role.Equals("Staff", StringComparison.OrdinalIgnoreCase))
            {
                return RedirectToAction("Kitchen");
            }

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult ManageMenu()
        {
            var menuItems = _context.MenuItems.OrderBy(m => m.DishId).ToList();
            return View(menuItems);
        }

        public IActionResult EditDish(int id)
        {
            var dish = _context.MenuItems
                .FromSqlRaw("SELECT * FROM MENU_ITEMS WHERE MI_DISHID = {0}", id)
                .AsEnumerable()
                .FirstOrDefault();

            if (dish == null) return NotFound();

            ViewBag.Inventory = _context.Inventory
                .FromSqlRaw("SELECT * FROM INVENTORY")
                .ToList();

            ViewBag.ExistingRecipes = _context.Recipes
                .FromSqlRaw("SELECT * FROM RECIPES WHERE MI_DISHID = {0}", id)
                .ToList();

            return View(dish);
        }

        [HttpPost]
        public IActionResult EditDish(MenuItem dish, int[] ingredients, decimal[] qtys)
        {
            try
            {
                _context.Database.ExecuteSqlRaw(
                    "UPDATE MENU_ITEMS SET MI_NAME = {0}, MI_CATEGORY = {1}, MI_PRICE = {2}, MI_IMAGEURL = {3} WHERE MI_DISHID = {4}",
                    dish.Name, dish.Category, dish.Price, dish.ImageUrl, dish.DishId
                );

                _context.Database.ExecuteSqlRaw("DELETE FROM RECIPES WHERE MI_DISHID = {0}", dish.DishId);

                if (ingredients != null && ingredients.Length > 0)
                {
                    int currentMaxId = (_context.Recipes.Any() ? _context.Recipes.Max(r => r.RecipeId) : 0);

                    for (int i = 0; i < ingredients.Length; i++)
                    {
                        if (ingredients[i] > 0 && qtys[i] > 0)
                        {
                            currentMaxId++;
                            _context.Database.ExecuteSqlRaw(
                                "INSERT INTO RECIPES (R_RECIPEID, MI_DISHID, I_INGREDIENTID, R_QTYREQUIRED) VALUES ({0}, {1}, {2}, {3})",
                                currentMaxId, dish.DishId, ingredients[i], qtys[i]
                            );
                        }
                    }
                }
                return RedirectToAction("ManageMenu");
            }
            catch (Exception ex)
            {
                ViewBag.Error = "Database Error: " + ex.Message;

                ViewBag.Inventory = _context.Inventory.ToList(); 

                ViewBag.ExistingRecipes = _context.Recipes
                    .Where(r => r.DishId == dish.DishId).ToList();

                return View(dish);
            }
        }
        [HttpPost]
        public IActionResult DeleteDish(int id)
        {
            var dish = _context.MenuItems.FirstOrDefault(m => m.DishId == id);
            if (dish != null)
            {
                try
                {
                    _context.MenuItems.Remove(dish);
                    _context.SaveChanges();
                }
                catch
                {
                    TempData["Error"] = "Cannot delete this dish because it is linked to past orders.";
                }
            }
            return RedirectToAction("ManageMenu");
        }
        [HttpGet]
        public IActionResult Inventory()
        {
           
          
            var stockData = _context.RawMaterials
                                    .FromSqlRaw("SELECT * FROM RAW_MATERIALS")
                                    .ToList();

            var sortedStock = stockData
                .OrderBy(r => r.CurrentStock <= r.MinReorder ? 0 : 1)
                .ThenBy(r => r.Name)
                .ToList();

            return View(sortedStock);
        }
        [HttpPost]
        public IActionResult MarkDone([FromBody] int orderId)
        {
            try
            {
                var order = _context.SalesHeaders.ToList().FirstOrDefault(s => s.BillId == orderId);

                if (order == null) return Json(new { success = false });

                order.Status = "Completed";

                var table = _context.RestaurantTables.ToList().FirstOrDefault(t => t.TableId == order.TableId);
                if (table != null) table.Status = "Available";

                _context.SaveChanges();
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false });
            }
        }
        [HttpGet]
        public IActionResult GetSalesAnalytics()
        {
            var sevenDaysAgo = DateTime.Now.Date.AddDays(-7);

            var dailyRevenue = _context.SalesHeaders
                .Where(s => s.Date >= sevenDaysAgo)
                .ToList() 
                .GroupBy(s => s.Date.Date)
                .Select(g => new {
                    SortDate = g.Key,
                    Date = g.Key.ToString("MMM dd"),
                    Total = g.Sum(s => s.TotalAmount)
                })
                .OrderBy(x => x.SortDate) 
                .ToList();

            // 2. Top 5 Dishes
            var topDishes = _context.SalesDetails
                .Join(_context.MenuItems,
                      sd => sd.DishId,
                      mi => mi.DishId,
                      (sd, mi) => new { mi.Name, sd.QtySold })
                .GroupBy(x => x.Name)
                .Select(g => new {
                    DishName = g.Key,
                    QuantitySold = g.Sum(x => x.QtySold)
                })
                .ToList()
                .OrderByDescending(x => x.QuantitySold)
                .Take(5)
                .ToList();

            return Json(new { dailyRevenue, topDishes });
        }
        [HttpGet]
        public IActionResult Tables()
        {
            var tables = _context.RestaurantTables.OrderBy(t => t.TableId).ToList();
            return View(tables);
        }

        [HttpPost]
        public IActionResult UpdateStock(int id, decimal addQty)
        {
            
            var item = _context.RawMaterials
                               .ToList()
                               .FirstOrDefault(rm => rm.Id == id);

            if (item != null)
            {
                item.CurrentStock += addQty;
                _context.SaveChanges();
            }
            return RedirectToAction("Inventory");
        }
        [HttpPost]
        public IActionResult Checkout([FromBody] CheckoutRequest request)
        {
            using (var transaction = _context.Database.BeginTransaction())
            {
                try
                {
                    if (request.Cart == null || request.Cart.Count == 0)
                        return Json(new { success = false, message = "Cart is empty" });

                    int newBillId = 1;
                    var maxId = _context.SalesHeaders.Select(s => (int?)s.BillId).Max();
                    if (maxId.HasValue) newBillId = maxId.Value + 1;

                    _context.Database.ExecuteSqlRaw(
                        "INSERT INTO SALES_HEADER (SH_BILLID, SH_DATE, SH_TOTALAMOUNT, SH_ORDERTYPE, RT_TABLEID, U_USERID, SH_STATUS) " +
                        "VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6})",
                        newBillId, DateTime.Now, request.Cart.Sum(i => i.Price * i.Qty), "Dine-in", request.TableId, 1, "Pending"
                    );

                    int nextDetailId = 1;
                    var maxDetail = _context.SalesDetails.Select(d => (int?)d.DetailId).Max();
                    if (maxDetail.HasValue) nextDetailId = maxDetail.Value + 1;

                    foreach (var cartItem in request.Cart)
                    {
                        _context.Database.ExecuteSqlRaw(
                            "INSERT INTO SALES_DETAILS (SD_DETAILID, SH_BILLID, MI_DISHID, SD_QTYSOLD) VALUES ({0}, {1}, {2}, {3})",
                            nextDetailId++, newBillId, cartItem.Id, cartItem.Qty
                        );

                        
                        var recipes = _context.Recipes.Where(r => r.DishId == cartItem.Id).ToList();

                        foreach (var recipe in recipes)
                        {
                            decimal amountToDeduct = cartItem.Qty * recipe.QuantityRequired;

                            _context.Database.ExecuteSqlRaw(
                                "UPDATE RAW_MATERIALS SET RM_CURRENTSTOCK = RM_CURRENTSTOCK - {0} WHERE RM_ID = {1}",
                                amountToDeduct, recipe.RawMaterialId
                            );
                        }
                    }

                    transaction.Commit();
                    return Json(new { success = true, orderId = newBillId });
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    var msg = ex.InnerException?.Message ?? ex.Message;
                    return Json(new { success = false, message = "Oracle Error: " + msg });
                }
            }
        }
        public class CheckoutRequest
        {
            public List<HomeController.CartItemDto> Cart { get; set; }
            public int TableId { get; set; }
        }

        public class CartItemDto
        {
            public int Id { get; set; }
            public string Name { get; set; }
            public decimal Price { get; set; }
            public int Qty { get; set; }
        }

        public class DishWithIngredientsViewModel
        {
            public MenuItem NewDish { get; set; }
            public List<IngredientSelection> SelectedIngredients { get; set; }
        }

        public class IngredientSelection
        {
            public int MaterialId { get; set; }
            public decimal Qty { get; set; }
        }
    }
   
}