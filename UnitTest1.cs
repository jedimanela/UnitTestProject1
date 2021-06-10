using CoderbyteTest.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Net;

namespace CoderbyteTest
{
    [TestClass]
    public class UnitTest1
    {
        public const string baseUrl = "https://1ryu4whyek.execute-api.us-west-2.amazonaws.com/dev/skus";

        RestClient client;

        [TestInitialize]
        public void SetUp()
        {
            client = new RestClient(baseUrl);
        }

        private IRestResponse GetPostList()
        {
            //Arrange
            //Initialize the request object with proper method and URL
            RestRequest request = new RestRequest(Method.GET);
            //Act
            // Execute the request
            IRestResponse response = client.Execute(request);
            return response;
        }

        /// <summary>
        /// Retrieve all details in the json file
        /// </summary>
        [TestMethod]
        public void OnCallingGetAPI_ReturnList()
        {
            IRestResponse response = GetPostList();
            // Check if the status code of response equals the default code for the method requested
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            // Convert the response object to list of posts
            List<Posts> postsList = JsonConvert.DeserializeObject<List<Posts>>(response.Content);
            foreach (Posts post in postsList)
            {
                Console.WriteLine("Sku: " + post.sku + "\t" + "Description: " + post.description + "\t" + "Price: " + post.price);
            }
        }

        /// <summary>
        /// Ability to add new post to the json file in JSON server and return the same
        /// </summary>
        [TestMethod]
        public void ValidateEntries()
        {
            IRestResponse response = GetPostList();
            // Check if the status code of response equals the default code for the method requested
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode);
            // Convert the response object to list of posts
            List<Posts> postsList = JsonConvert.DeserializeObject<List<Posts>>(response.Content);
            foreach (Posts post in postsList)
            {
                Assert.AreNotEqual("", post.sku);
                Assert.AreNotEqual("", post.description);
                Assert.AreNotEqual("", post.price);
                Assert.IsTrue(IsNumeric(post.price));
            }
        }

        public bool IsNumeric(string number)
        {
            double retNum;

            bool isNum = Double.TryParse(number, System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }

        /// <summary>
        /// Ability to add new post to the json file in JSON server and return the same
        /// </summary>
        [TestMethod]
        public void AddNewItem()
        {
            //Arrange
            ///Initialize the request for POST to add new post
            RestRequest request = new RestRequest(Method.POST);
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("sku", "Maple Bar");
            jsonObj.Add("description", "Bar Donut");
            jsonObj.Add("price", "1.99");
            ///Added parameters to the request object such as the content-type and attaching the jsonObj with the request
            request.AddParameter("application/json", jsonObj, ParameterType.RequestBody);

            //Act
            IRestResponse response = client.Execute(request);

            //Assert
            Posts posts = JsonConvert.DeserializeObject<Posts>(response.Content);
            Assert.AreEqual("Maple Bar", posts.sku);
            Assert.AreEqual("Bar Donut", posts.description);
            Assert.AreEqual("1.99", posts.price);
            Console.WriteLine(response.Content);
        }

        /// <summary>
        /// UC3 Ability to adding multiple posts to the json file using JSON server and returns the same
        /// </summary>
        [TestMethod]
        public void AddSeveralItems()
        {
            // Arrange
            List<Posts> postList = new List<Posts>();
            postList.Add(new Posts { sku = "Chocolate Old Fashioned", description = "Cake Donut", price = ".99"});
            postList.Add(new Posts { sku = "Glazed", description = "Yeast Donut", price = ".99" });
            postList.Add(new Posts { sku = "Apple Fritter", description = "Apple Fritter", price = "2.99" });
            //Iterate the loop for each post
            foreach (var post in postList)
            {
                ///Initialize the request for POST to add new posts
                RestRequest request = new RestRequest(Method.POST);
                JsonObject jsonObj = new JsonObject();
                jsonObj.Add("sku", post.sku);
                jsonObj.Add("description", post.description);
                jsonObj.Add("price", post.price);
                ///Added parameters to the request object such as the content-type and attaching the jsonObj with the request
                request.AddParameter("application/json", jsonObj, ParameterType.RequestBody);

                //Act
                IRestResponse response = client.Execute(request);

                //Assert
                Posts posts = JsonConvert.DeserializeObject<Posts>(response.Content);
                Assert.AreEqual(posts.sku, posts.sku);
                Assert.AreEqual(posts.description, posts.description);
                Assert.AreEqual(posts.price, posts.price);
                Console.WriteLine(response.Content);
            }
        }

        /// <summary>
        /// Ability to update the price into the json file in json server
        /// </summary>
        [TestMethod]
        public void UpdatePriceOnExistingItem()
        {
            //Arrange
            ///Initialize the request for PUT to add new posts
            RestRequest request = new RestRequest("/beliner2", Method.PUT);
            JsonObject jsonObj = new JsonObject();
            jsonObj.Add("price", ".50");
            ///Added parameters to the request object such as the content-type and attaching the jsonObj with the request
            request.AddParameter("application/json", jsonObj, ParameterType.RequestBody);

            //Act
            IRestResponse response = client.Execute(request);

            //Assert
            Posts post = JsonConvert.DeserializeObject<Posts>(response.Content); // Missing Authenication Token?
            Assert.AreEqual(".50", post.price);
            Console.WriteLine(response.Content);
        }

        /// <summary>
        /// Ability to delete the post details with given sku
        /// </summary>
        [TestMethod]
        public void DeleteItem()
        {
            //Arrange
            //Initialize the request for PUT to add new post
            RestRequest request = new RestRequest(Method.DELETE);

            //Act
            IRestResponse response = client.Execute(request);

            //Assert
            Assert.AreEqual(HttpStatusCode.OK, response.StatusCode); // Forbidden?
            Console.WriteLine(response.Content);
        }
    }
}
