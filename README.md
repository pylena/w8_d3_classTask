# OverView
This project demonstrates how to build a .NET Web API that fetches data from an online source (JSONPlaceholder), implements both **in-memory (IMemoryCache)** and **Redis caching**, and provides a simple HTML frontend that dynamically fetches and displays posts using the **Fetch API** with **lazy-loaded images**.


## Part 1: Fetch Data from Online API using Redis

- **Objective:** Fetch data from the JSONPlaceholder API (https://jsonplaceholder.typicode.com/posts).
- **Implementation:**
  - Used `HttpClientFactory` to send GET requests.
  - Created a `PostsController` to expose the `/api/posts` endpoint.
  - Deserialized and returned the data as JSON.
  
