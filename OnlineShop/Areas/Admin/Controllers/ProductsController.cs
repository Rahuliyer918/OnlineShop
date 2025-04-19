using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using OnlineShop.Models.Db;

namespace OnlineShop.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly OnlineShopContext _context;

        public ProductsController(OnlineShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Products
        public async Task<IActionResult> Index()
        {
            return View(await _context.Products.ToListAsync());
        }

        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["gallery"] = _context.ProductsGalleries.Where(gallery => gallery.ProductId == id).ToList();
            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,FullDesc,Price,Discount,ImageName,Qty,Tags,VideoUrl")] Product product, IFormFile? MainImage, IFormFile[]? GalleryImages)
        {
            if (ModelState.IsValid)
            {

                //Save Main Image
                if (MainImage != null)
                {
                    product.ImageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(MainImage.FileName);
                    string fn;
                    fn = Directory.GetCurrentDirectory();
                    string ImagePath = fn + "\\wwwroot\\images\\products\\" + product.ImageName;

                    using (var stream = new FileStream(ImagePath, FileMode.Create))
                    {
                        MainImage.CopyTo(stream);
                    }
                }

                _context.Add(product);
                await _context.SaveChangesAsync();

                if (GalleryImages != null)
                {
                    foreach (var image in GalleryImages)
                    {
                        var newGallery = new ProductsGallery();
                        newGallery.ProductId = product.Id;
                        newGallery.ImageName = Guid.NewGuid().ToString() + System.IO.Path.GetExtension(image.FileName);
                        string fn = Directory.GetCurrentDirectory();
                        string ImagePath = fn + "\\wwwroot\\images\\products\\" + newGallery.ImageName;

                        using (var stream = new FileStream(ImagePath, FileMode.Create))
                        {
                            image.CopyTo(stream);
                        }

                        _context.ProductsGalleries.Add(newGallery);
                    }
                }

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            ViewData["gallery"] = _context.ProductsGalleries.Where(gallery => gallery.ProductId == id).ToList();
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,FullDesc,Price,Discount,ImageName,Qty,Tags,VideoUrl")] Product product, IFormFile? MainImage, IFormFile[]? GalleryImages)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    if (MainImage != null)
                    {
                        string d = Directory.GetCurrentDirectory();
                        string fn = d + "\\wwwroot\\images\\products\\" + product.ImageName;
                        //------------------------------------------------
                        if (System.IO.File.Exists(fn))
                        {
                            System.IO.File.Delete(fn);
                        }

                        //string ImagePath;
                        //ImagePath = Directory.GetCurrentDirectory() + "/wwwroot/images/products/" + product.ImageName;

                        //------------------------------------------------
                        using (var stream = new FileStream(fn, FileMode.Create))
                        {
                            MainImage.CopyTo(stream);
                        }
                        //------------------------------------------------
                    }                                     

                    if (GalleryImages != null)
                    {
                        foreach (var item in GalleryImages)
                        {

                            var imageName = Guid.NewGuid() + Path.GetExtension(item.FileName);
                            //------------------------------------------------
                            string d = Directory.GetCurrentDirectory();
                            string fn = d + "\\wwwroot\\images\\products\\" + imageName;
                            //------------------------------------------------
                            using (var stream = new FileStream(fn, FileMode.Create))
                            {
                                item.CopyTo(stream);
                            }
                            //------------------------------------------------
                            var galleryItem = new ProductsGallery();
                            galleryItem.ImageName = imageName;
                            galleryItem.ProductId = product.Id;
                            //------------------------------------------------
                            _context.ProductsGalleries.Add(galleryItem);
                        }
                    }

                    _context.Update(product);
                    await _context.SaveChangesAsync();

                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .FirstOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                string d = Directory.GetCurrentDirectory();
                string fn = d + "\\wwwroot\\images\\products\\";
                //----------
                string mainImagePath = fn + product.ImageName;
                //--------------------------
                if (System.IO.File.Exists(mainImagePath))
                {
                    System.IO.File.Delete(mainImagePath);
                }
                //--------------------------
                var galleries = _context.ProductsGalleries.Where(x => x.ProductId == id).ToList();
                if (galleries != null)
                {
                    //--------------------------
                    foreach (var item in galleries)
                    {
                        string galleryImagePath = fn + item.ImageName;
                        //--------------------------
                        if (System.IO.File.Exists(galleryImagePath))
                        {
                            System.IO.File.Delete(galleryImagePath);
                        }
                        //--------------------------
                    }
                    //--------------------------
                    _context.ProductsGalleries.RemoveRange(galleries);
                }
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        /// <summary>
        /// Delete Images from Gallery Images
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public IActionResult DeleteGallery(int id)
        {
            var gallery = _context.ProductsGalleries.FirstOrDefault(x => x.Id == id);
            if (gallery == null)
            {
                return NotFound();
            }
            string d = Directory.GetCurrentDirectory();
            string fn = d + "\\wwwroot\\images\\products\\" + gallery.ImageName;

            if (System.IO.File.Exists(fn))
            {
                System.IO.File.Delete(fn);
            }
            _context.Remove(gallery);
            _context.SaveChanges();

            return Redirect("edit/" + gallery.ProductId);
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }
    }
}
