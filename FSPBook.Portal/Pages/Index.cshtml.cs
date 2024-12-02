using FSPBook.Data;
using FSPBook.Data.Entities;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace FSPBook.Pages;
public class IndexModel : PageModel
{
    public PostViewModel PostViewModel { get; set; }

    private readonly Context DbContext;
    private readonly HttpClient _httpClient;

    private const string ApiUrl = "https://api.thenewsapi.com/v1/news/all";

    public IndexModel(Context context, HttpClient httpClient)
    {
        DbContext = context;
        _httpClient = httpClient;
    }

    public async Task OnGet(int pageNumber = 1)
    {
        int pageSize = 10;

        try
        {
            var posts = DbContext.Post
                .Include(post => post.Author)
                .OrderByDescending(post => post.DateTimePosted)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var hasMorePosts = DbContext.Post.Count() > pageNumber * pageSize;

            var feeds = await GetFeeds();
             
            PostViewModel = new PostViewModel
            {
                Posts = posts,
                Feeds = feeds,
                HasMorePosts = hasMorePosts
            };
        }
        catch(Exception ex)
        {
            Console.Error.WriteLine($"Error fetching posts: {ex.Message}");

            PostViewModel = new PostViewModel
            {
                Posts = null,
                HasMorePosts = false
            };
        }
    }

    public virtual async Task<List<Feed>> GetFeeds()
    {
        try
        {
            var queryString = new List<string>
                {
                    "api_token=EnectupEUdWmOx7IPUq7L3zIF5OvuKfE2uqUtitL",
                    "language=en",
                    "limit=5"
                };

            var urlWithParams = $"{ApiUrl}?{string.Join("&", queryString)}";

            var response = await _httpClient.GetAsync(urlWithParams);

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(content);
                
                return apiResponse.Data;
            }
            else
            {
                throw new Exception("Error occur while fetching feeds.");
            }
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error fetching feeds: {ex.Message}");

            return null;
        }
    }
}

public class PostViewModel
{
    public List<Post> Posts { get; set; }

    public List<Feed> Feeds { get; set; }

    public bool HasMorePosts { get; set; }
}

public class ApiResponse
{
    public List<Feed> Data { get; set; }
}

public class Feed
{
    public string Title { get; set; }
    public string Description { get; set; }
    public string Url { get; set; }
    public string ImageUrl { get; set; }
    public string PublishedAt { get; set; }
    public string Source { get; set; }
    public List<string> Categories { get; set; }
}