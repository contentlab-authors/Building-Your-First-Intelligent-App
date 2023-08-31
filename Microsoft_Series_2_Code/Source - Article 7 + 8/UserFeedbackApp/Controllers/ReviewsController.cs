using Azure.AI.TextAnalytics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using UserFeedbackApp.Models;

namespace UserFeedbackApp.Controllers
{
    public class ReviewsController : Controller
    {
        private readonly DatabaseContext _context;
        private readonly TextAnalyticsClient _textClient;

        public ReviewsController(DatabaseContext context, TextAnalyticsClient textClient)
        {
            _context = context;
            _textClient = textClient;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var reviewList = await _context.Reviews.ToListAsync();
            var productList = await _context.Products.ToListAsync();

            List<SentimentTrends> trends = new List<SentimentTrends>();

            foreach(var product in productList)
            {
                var laptopReviews = reviewList
                .Where(d => d.ProductId == product.Id)
                .Where(d => DateTime.ParseExact(d.PostDate, "yyyy-MM-dd", CultureInfo.InvariantCulture) >= (DateTime.Now.AddDays(-8)))
                .ToList();

                if (laptopReviews.Count() > 0)
                {

                    float positiveValue = 0;
                    float neutralValue = 0;
                    float negativeValue = 0;

                    foreach (var review in laptopReviews)
                    {
                        positiveValue += review.PositiveValue;
                        neutralValue += review.NeutralValue;
                        negativeValue += review.NegativeValue;
                    }

                    positiveValue = (float)Math.Round(positiveValue / laptopReviews.Count(), 2);
                    neutralValue = (float)Math.Round(neutralValue / laptopReviews.Count(), 2);
                    negativeValue = (float)Math.Round(negativeValue / laptopReviews.Count(), 2);

                    SentimentTrends trend = new SentimentTrends();

                    trend.Id = laptopReviews.First().ProductId;
                    trend.ProductName = laptopReviews.First().ProductName;
                    trend.PostDate = DateTime.Now.AddDays(-8).ToString("yyyy-MM-dd");
                    trend.PositiveValue = positiveValue;
                    trend.NeutralValue = neutralValue;
                    trend.NegativeValue = negativeValue;

                    trends.Add(trend);
                }
            }

            ViewBag.Trends = trends;
            return reviewList != null ? 
                    View(reviewList) :
                    Problem("Entity set 'DatabaseContext.Reviews'  is null.");
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public async Task<IActionResult> Create()
        {
            var products = await _context.Products.ToListAsync<Product>();
            List<SelectListItem> productSelections = new List<SelectListItem>();

            foreach (var product in products)
            {
                productSelections.Add(new SelectListItem { Text = product.Name, Value = product.Id.ToString() });
            }

            ViewBag.Products = productSelections;

            return View();
        }

        // POST: Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductId,ProductName,ReviewText,PostDate,Sentiment,PositiveValue,NeutralValue,NegativeValue")] Review review)
        {
            var selectedProductId = int.Parse(HttpContext.Request.Form["Products"].ToString());
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == selectedProductId);

            var sentimentResult = await _textClient.AnalyzeSentimentAsync(review.ReviewText);
            
            review.ProductId = selectedProductId;
            review.ProductName = product.Name;
            review.PostDate = DateTime.Now.ToString("yyyy-MM-dd");
            review.Sentiment = sentimentResult.Value.Sentiment.ToString();
            review.PositiveValue = (float)sentimentResult.Value.ConfidenceScores.Positive;
            review.NeutralValue = (float)sentimentResult.Value.ConfidenceScores.Neutral;
            review.NegativeValue = (float)sentimentResult.Value.ConfidenceScores.Negative;

            _context.Add(review);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            return View(review);
        }

        // POST: Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductId,ProductName,ReviewText,PostDate,Sentiment,PositiveValue,NeutralValue,NegativeValue")] Review review)
        {
            if (id != review.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.Id))
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
            return View(review);
        }

        // GET: Reviews/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Reviews == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .FirstOrDefaultAsync(m => m.Id == id);
            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Reviews == null)
            {
                return Problem("Entity set 'DatabaseContext.Reviews'  is null.");
            }
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
          return (_context.Reviews?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
