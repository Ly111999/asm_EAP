using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using TravelServices.Models;

namespace TravelServices.Controllers
{
    [RoutePrefix("api/v1/guide")]
    public class PostController : ApiController
    {
        TravellerContext db = new TravellerContext();
        List<Tag> listTags = new List<Tag>();
        List<Tag> listCurrentTags = new List<Tag>();

        [Route("post/add")]
        [HttpPost]
        public HttpResponseMessage AddPosts([FromBody] BodyPostMe parameters)
        {

            if (ModelState.IsValid)
            {
                parameters.Post.createdAt = DateTime.Now;
                parameters.Post.updatedAt = DateTime.Now;
                db.Posts.Add(parameters.Post);
                db.SaveChanges();
            }

            listTags = parameters.Tag;
            for (int i = 0; i < listTags.Count; i++)
            {
                if (!TagExists(listTags[i].tag_name))
                {
                    db.Tags.Add(listTags[i]);
                    db.SaveChanges();
                }
            }

            var currentPost = db.Posts.Where(a => a.Traveler_id == parameters.Post.Traveler_id).OrderByDescending(b => b.id).FirstOrDefault();

            for (int z = 0; z < listTags.Count; z++)
            {
                var tagName = listTags[z].tag_name;
                var listTag = db.Tags.Where(t => t.tag_name == tagName).OrderByDescending(b => b.id).FirstOrDefault();
                Debug.WriteLine(listTag.id);
                listCurrentTags.Add(listTag);
            }

            Tag_Post tp = new Tag_Post();
            for (int m = 0; m < listCurrentTags.Count; m++)
            {
                tp.Post_id = currentPost.id;
                tp.Tag_id = listCurrentTags[m].id;
                db.Tag_Post.Add(tp);
                db.SaveChanges();
            }


            return Request.CreateResponse(HttpStatusCode.OK, currentPost);
        }

        [Route("post")]
        [HttpGet]
        public List<Post> GetAllPosts()
        {
            return db.Posts.ToList();
        }

        [Route("post/{Traveler_id}")]
        [HttpGet]
        public List<Post> GetAllGuidePosts(int Traveler_id)
        {
            return db.Posts.Where(a => a.Traveler_id == Traveler_id).ToList();
        }

        [Route("post/{id}")]
        [HttpGet]
        public HttpResponseMessage GetPostsById(int id)
        {
            Post post = db.Posts.Where(a => a.id == id).FirstOrDefault();
            if (post == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Post not found.");
            }
            else
            {
                return Request.CreateResponse(HttpStatusCode.OK, post);
            }
        }

        [Route("post/edit/{id}")]
        [HttpPut]
        public HttpResponseMessage EditById(int id, Post post)
        {
            if (!ModelState.IsValid)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, ModelState);
            }

            if (id != post.id)
            {
                return Request.CreateErrorResponse(HttpStatusCode.BadRequest, "Can not edit post id.");
            }

            db.Entry(post).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PostExists(id))
                {
                    return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Post not found.");
                }
                else
                {
                    throw;
                }
            }

            return Request.CreateErrorResponse(HttpStatusCode.OK, "Edit success.");
        }

        [Route("post/delete/{id}")]
        [HttpDelete]
        public HttpResponseMessage DeleteById(int id)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return Request.CreateErrorResponse(HttpStatusCode.NotFound, "Post not found.");
            }

            db.Posts.Remove(post);
            db.SaveChanges();

            return Request.CreateResponse(HttpStatusCode.OK, post);
        }


        private bool TagExists(string tagName)
        {
            return db.Tags.Count(e => e.tag_name == tagName) > 0;
        }

        private bool PostExists(int id)
        {
            return db.Posts.Count(e => e.id == id) > 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }


    }
}
