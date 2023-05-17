﻿using NLog;
using System.Linq;
using System.ComponentModel.DataAnnotations;




// See https://aka.ms/new-console-template for more information
string path = Directory.GetCurrentDirectory() + "\\nlog.config";

// create instance of Logger
var logger = LogManager.LoadConfiguration(path).GetCurrentClassLogger();
logger.Info("Program started");
try
{
string choice;
    do{

Console.WriteLine("Please chose an option, or any other button to close.");
Console.WriteLine("1. Display all blogs.");
Console.WriteLine("2. Add blog.");
Console.WriteLine("3. Create post.");
Console.WriteLine("4. Display Posts");
        choice = Console.ReadLine();
        Console.Clear();
        logger.Info("Option {choice} selected", choice);

        var db = new BloggingContext();

        if (choice == "1")
        {
            // Display all Blogs from the database
     
            var query = db.Blogs.OrderBy(b => b.Name);

            Console.WriteLine("All blogs in the database:");
            foreach (var item in query)
                {
                Console.WriteLine(item.Name);
                }
        }
        else if (choice == "2")
        {
         // Create and save a new Blog
    Console.Write("Enter a name for a new Blog: ");
            var name = Console.ReadLine();
            var blog = new Blog { Name = name };
            db.AddBlog(blog);
            logger.Info("Blog added - {name}", name);
       
        }
        if(choice == "3"){
            Console.WriteLine("Choose a blog to post in:");
            var blog = GetBlog(db, logger);
            if(blog != null){
                Console.WriteLine("Enter the Post Title");
                String title = Console.ReadLine();
                if(title != null){
                    Console.WriteLine("Enter the Post content");
                    String content = Console.ReadLine();
                    if(content != null){
                        var post = new Post {
                            Title = title,
                            Content = content,
                            BlogId = blog.BlogId,
                            Blog = blog
                        };
                        db.AddPost(post);
                        logger.Info($"Post added - '{content}'");
                    }
                    else{
                        logger.Error("Post cannot be null");
                    }
                }
                else{
                    logger.Error("Post title cannot be null");
                }
            
            }


         }

        if(choice == "4"){
            
         
            var blogs = db.Blogs.OrderBy(b => b.BlogId);

            Console.WriteLine("0) Posts from all blogs");
            foreach (Blog b in blogs)
            {
            Console.WriteLine($"{b.BlogId}) Posts from {b.Name}");
            }
            String answer = Console.ReadLine();
            if(answer == "0"){
                var query = db.Posts.OrderBy(b => b.Title);
                int num = db.Posts.Count();
                Console.WriteLine($"{num} post(s) returned");
                foreach (var item in query)
                    {
                    Console.WriteLine($"Blog: {item.Blog}\nTitle: {item.Title}\nContent: {item.Content}\n");
                    }
                
            }
            else{
            if (int.TryParse(answer, out int BlogId))
            {
            Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == BlogId);
                if (blog != null)
                {
                    var blogPost = db.Posts.Where(p => p.Blog.Name.Contains(blog.Name));
                    foreach(Post p in blogPost){
                        Console.WriteLine($"Blog: {p.Blog}\nTitle: {p.Title}\nContent: {p.Content}\n");
                    }
                    
                }
            }
            else{
                logger.Error("There's no blogs with that Id");
            }
    }
    logger.Error("Invalid Blog Id");

        }
   
        Console.WriteLine();
    } while (choice == "1" || choice == "2" || choice == "3" || choice == "4");
}
catch (Exception ex)
{
    logger.Error(ex.Message);
}


logger.Info("Program ended");

static Blog GetBlog(BloggingContext db, Logger logger)
{
    // display all blogs
    var blogs = db.Blogs.OrderBy(b => b.BlogId);
    foreach (Blog b in blogs)
    {
        Console.WriteLine($"{b.BlogId}: {b.Name}");
    }
    if (int.TryParse(Console.ReadLine(), out int BlogId))
    {
        Blog blog = db.Blogs.FirstOrDefault(b => b.BlogId == BlogId);
        if (blog != null)
        {
            return blog;
        }
    }
    logger.Error("Invalid Blog Id");
    return null;
}



