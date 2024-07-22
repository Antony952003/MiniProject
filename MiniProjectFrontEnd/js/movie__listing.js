document.addEventListener("DOMContentLoaded", () => {
  document.querySelector(".logo").addEventListener("click", () => {
    window.location.href = "index.html";
  });
  document.querySelector(".ad-nav-content").addEventListener("click", () => {
    window.location.href = "booking_orders.html";
  });

  const fetchuserdetails = () => {
    fetch(
      `http://localhost:5091/api/User/GetUserById?userid=${localStorage.getItem(
        "uid"
      )}`,
      {
        method: "GET",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    )
      .then((response) => response.json())
      .then((data) => {
        console.log(data);
        document.querySelectorAll(".name-field").forEach((e) => {
          e.innerHTML = data.name;
        });
        var imageeles = document.querySelectorAll(".user-profile-image");
        imageeles.forEach((imageele) => {
          imageele.src = data.image;
          imageele.onerror = function () {
            imageele.src = "../Assets/Images/user_image.png";
          };
        });
      });
  };
  fetchuserdetails();

  document.querySelector(".user").addEventListener("click", () => {
    if (!document.querySelector(".user").classList.contains("active")) {
      document.querySelector(".user").classList.add("active");
      document.querySelector(".account-details-nav").classList.add("active");
      if (document.querySelector("#nav-menu").classList.contains("active")) {
        document.querySelector("#nav-menu").classList.remove("active");
        document.querySelector("#hamburger-menu").classList.remove("active");
      }
    } else {
      document.querySelector(".user").classList.remove("active");
      document.querySelector(".account-details-nav").classList.remove("active");
    }
  });

  document
    .querySelector(".ad-nav-header-comp-2")
    .addEventListener("click", () => {
      if (!document.querySelector(".user").classList.contains("active")) {
        document.querySelector(".user").classList.add("active");
        document.querySelector(".account-details-nav").classList.add("active");
        if (document.querySelector("#nav-menu").classList.contains("active")) {
          document.querySelector("#nav-menu").classList.remove("active");
          document.querySelector("#hamburger-menu").classList.remove("active");
        }
      } else {
        document.querySelector(".user").classList.remove("active");
        document
          .querySelector(".account-details-nav")
          .classList.remove("active");
      }
    });

  if (localStorage.getItem("token")) {
    document.getElementById("logged-in").innerHTML = "Log Out";
  }

  document.getElementById("logged-in").addEventListener("click", () => {
    if (document.getElementById("logged-in").innerHTML == "Log Out") {
      document.getElementById("logged-in").innerHTML = "Sign In";
      window.localStorage.removeItem("token");
    }
  });

  document.querySelector(".hamburger-menu").addEventListener("click", () => {
    if (
      !document.querySelector(".hamburger-menu").classList.contains("active")
    ) {
      document.querySelector(".hamburger-menu").classList.add("active");
      document.querySelector(".nav-menu").classList.add("active");
    } else {
      document.querySelector(".hamburger-menu").classList.remove("active");
      document.querySelector(".nav-menu").classList.remove("active");
      if (
        document.querySelector(".popup-location").classList.contains("active")
      ) {
        document.querySelector(".popup-location").classList.remove("active");
        document.querySelector(".main-container").style.opacity = "1.0";
      }
    }
  });

  var locationdown = document.querySelector(".down-button");
  locationdown.addEventListener("click", () => {
    var popuplocation = document.querySelector(".popup-location");
    if (popuplocation.classList.contains("active")) {
      document.querySelector(".main-container").style.opacity = "1.0";
      popuplocation.classList.remove("active");
    } else {
      document.querySelector(".main-container").style.opacity = "0.3";
      popuplocation.classList.add("active");
    }
  });

  const cityButtons = document.querySelectorAll(".pcities button");
  const locationSpan = document.querySelector(".location span");

  cityButtons.forEach((button) => {
    button.addEventListener("click", () => {
      var popuplocation = document.querySelector(".popup-location");
      document.querySelector(".main-container").style.opacity = "1.0";
      popuplocation.classList.remove("active");
      locationSpan.innerHTML = button.innerHTML;
    });
  });
  const movieListContainer = document.getElementById("movie-list");

  loader.style.display = "block";
  movieListContainer.style.display = "none";

  const addMovieCardClickListeners = (movies) => {
    const movieCards = document.querySelectorAll(".movie-card");
    movieCards.forEach((card, index) => {
      card.addEventListener("click", () => {
        const movie = movies[index];
        window.location.href = `movie.html?title=${encodeURIComponent(
          movie.title
        )}`;
      });
    });
  };

  const fetchMovies = () => {
    fetch(`http://localhost:5091/api/Movie/GetAllMovies`, {
      method: "GET",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    })
      .then((response) => response.json())
      .then((data) => {
        console.log(data);
        loader.style.display = "none";
        movieListContainer.style.display = "flex";
        renderMovies(data); // Initial rendering of all movies

        // Filter event listeners
        document
          .getElementById("genre-filter")
          .addEventListener("change", () => {
            filterMovies(data);
          });

        document
          .getElementById("certification-filter")
          .addEventListener("change", () => {
            filterMovies(data);
          });
      })
      .catch((error) => {
        console.error("Error fetching movies:", error);
      });
  };

  const renderMovies = (movies) => {
    movieListContainer.innerHTML = movies
      .map((movie) => {
        return `
          <div class="movie-card">
            <div class="movie-image">
              <img src="${movie.mainImage}" alt="${movie.title}" />
            </div>
            <div class="movie-card-content">
              <h3>${movie.title}</h3>
              <div class="movie-info">
                <p>${movie.certification}</p>
                <p>English,Tamil,Malayalam,Telugu,Kannada</p>
              </div>
            </div>
          </div>
        `;
      })
      .join("");
    addMovieCardClickListeners(movies);
  };

  const filterMovies = (movies) => {
    const genreFilter = document.getElementById("genre-filter").value;
    const certificationFilter = document.getElementById(
      "certification-filter"
    ).value;

    const filteredMovies = movies.filter((movie) => {
      // Apply genre filter
      if (genreFilter && genreFilter !== "") {
        const genres = movie.genre.split(",").map((g) => g.trim()); // Split genres by comma and trim spaces
        if (!genres.includes(genreFilter)) {
          return false;
        }
      }

      if (certificationFilter && certificationFilter !== "") {
        if (movie.certification !== certificationFilter) {
          return false;
        }
      }

      return true; // Include the movie in the filtered list
    });
    console.log(filteredMovies);
    if (filteredMovies.length === 0) {
      document.getElementById("noresult").style.display = "block";
    } else {
      document.getElementById("noresult").style.display = "none";
    }

    renderMovies(filteredMovies); // Render the filtered movies
  };

  // Initial fetch and render
  fetchMovies();
});
