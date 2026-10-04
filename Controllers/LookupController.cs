using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using HouseholdExpenseTrackerAPI.Services;
using HouseholdExpenseTrackerAPI.Dtos;

namespace HouseholdExpenseTrackerAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LookupController : ControllerBase
    {
        private readonly ILookupService _lookupService;

        public LookupController(ILookupService lookupService)
        {
            _lookupService = lookupService;
        }

        // GET api/lookup/income-categories
        [HttpGet("income-categories")]
        public async Task<ActionResult<List<DropdownItemDto>>> GetIncomeCategories()
        {
            return Ok(await _lookupService.GetIncomeCategoriesAsync());
        }

        // GET api/lookup/income-sources
        [HttpGet("income-sources")]
        public async Task<ActionResult<List<DropdownItemDto>>> GetIncomeSources()
        {
            return Ok(await _lookupService.GetIncomeSourcesAsync());
        }

        // GET api/lookup/expense-categories
        [HttpGet("expense-categories")]
        public async Task<ActionResult<List<DropdownItemDto>>> GetExpenseCategories()
        {
            return Ok(await _lookupService.GetExpenseCategoriesAsync());
        }

        // GET api/lookup/expense-subcategories?expenseCategoryId=3

        [HttpGet("expense-subcategories")]
        public async Task<IActionResult> GetExpenseSubCategories([FromQuery] int expenseCategoryId)
        {
            if (expenseCategoryId <= 0)
                return BadRequest("expenseCategoryId is required.");

            return Ok(await _lookupService.GetExpenseSubCategoriesAsync(expenseCategoryId));
        }

        [HttpGet("expense-items")]
        public async Task<IActionResult> GetExpenseItems([FromQuery] int expenseSubCategoryId)
        {
            if (expenseSubCategoryId <= 0)
                return BadRequest("expenseSubCategoryId is required.");

            return Ok(await _lookupService.GetExpenseItemsAsync(expenseSubCategoryId));
        }


        // GET api/lookup/roles
        [HttpGet("roles")]
        public async Task<ActionResult<List<DropdownItemDto>>> GetRoles()
        {
            return Ok(await _lookupService.GetRolesAsync());
        }
    }
}