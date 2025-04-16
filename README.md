# OverView
This project demonstrates how to build a .NET Web API that fetches data from an online source (JSONPlaceholder), implements both **in-memory (IMemoryCache)** and **Redis caching**, and provides a simple HTML frontend that dynamically fetches and displays posts using the **Fetch API** with **lazy-loaded images**.


## Fetch Data from Online API using Redis

- **Objective:** Fetch data from the JSONPlaceholder API (https://jsonplaceholder.typicode.com/posts).
- **Implementation:**
  - Used `HttpClientFactory` to send GET requests.
  - Created a `PostsController` , 'UsersController' to expose the `/api/posts` endpoint.
  - Deserialized and returned the data as JSON.

#  Implement Caching using In-Memory & Redis
- IMemoryCache (In-Memory Caching)
  Used IMemoryCache.TryGetValue and Set methods.
  Cached API response for 5 minutes (AbsoluteExpiration) with a 2-minute sliding window.

  - Redis Caching
    Connected to a Redis instance.
    If data not found, fetched from JSONPlaceholder and saved in Redis with expiration.

## Dynamic UI Updates with JavaScript
Frontend: Basic posts.html, users.html file
- Functionality:
* Uses the Fetch API to call the backend /api/posts, Users endpoint.
* Dynamically populates the DOM with post,users titles and bodies.
* UI updates without page reload.

### Image Optimization with Lazy Loading
Images: Fetched from https://picsum.photos
- Implementation:
* Used loading="lazy" on <img> tags.
* Images only load when they enter the viewport.



  
