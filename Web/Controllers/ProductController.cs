using Application.DTO;
using Application.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
	public class ProductController : Controller
	{

	
		private readonly IProducts iproduct;
		private readonly ICategory icat;
		public ProductController( IProducts _iproduct,ICategory _icat)
		{
			iproduct = _iproduct;
			icat = _icat;
		}

		public async Task<ActionResult> ProductList()
		{
			return View();
		}

		public async Task<IActionResult> AddProduct(int? id)
		{
			// این متد باید ViewBagها را پر کند
			var res = await icat.GetAll();
			ViewBag.category = await icat.GetAll();

			// حالت افزودن محصول جدید
			if (id == null || id == 0)
			{
				// >> مهمترین بخش <<
				// اینجا یک شیء خالی ولی غیر-null به ویو پاس داده می‌شود.
				// نام ویو "AddProduct" است تا با فایل شما مطابقت داشته باشد.
				return View("AddProduct", new CreateProductsDTO());
			}

			// حالت ویرایش محصول موجود
			var productDto = await iproduct.GetForEdit(id.Value);
			if (productDto == null)
			{
				return NotFound();
			}

			// مدل پر شده از دیتابیس به ویو پاس داده می‌شود.
			return View("AddProduct", productDto);
		}

		[HttpPost]
		public async Task<IActionResult> AddProduct(CreateProductsDTO dto)
		{
			if (!ModelState.IsValid)
			{

				// FIX: نام ویو را به صراحت مشخص کنید
				return View("AddProduct", dto);
			}
			if (dto.Id == 0)
			{
				int newId = await iproduct.CreateProductAsync(dto);
				return RedirectToAction(nameof(AddProduct), new { id = newId });
			}
			else
			{
				await iproduct.Update(dto);
				return RedirectToAction(nameof(AddProduct), new { id = dto.Id });
			}
		}


	}
}
