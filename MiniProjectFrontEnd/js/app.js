$(document).ready(() => {
  $("#hamburger-menu").click(() => {
    $("#hamburger-menu").toggleClass("active");
    $("#nav-menu").toggleClass("active");
  });
  const decodeToken = (token) => {
    return jwt_decode(token);
  };

  // Function to check if the token is expired
  const isTokenExpired = (token) => {
    const decodedToken = decodeToken(token);
    const currentTime = Date.now() / 1000; // Current time in seconds
    return decodedToken.exp < currentTime;
  };
  if (isTokenExpired(localStorage.getItem("token"))) {
    localStorage.removeItem("token");
    window.location.href = "login.html";
  }

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

  document.getElementById("edit-profile").addEventListener("click", () => {
    window.location.href = "EditProfile.html";
  });

  function createSlideItem(movie) {
    const heroslideitem = document.createElement("div");
    heroslideitem.classList.add("hero-slide-item");

    heroslideitem.innerHTML = `
          
            <img src="${movie.bannerImage}" alt="${movie.title}" />
            <div class="overlay"></div>
            <div class="hero-slide-item-content">
                <div class="item-content-wraper">
                    <div class="item-content-title top-down">${
                      movie.title
                    }</div>
                    <div class="movie-infos top-down delay-2">
                        <div class="movie-info">
                            <i class="bx bxs-star"></i>
                            <span>${movie.averageRating}</span>
                        </div>
                        <div class="movie-info">
                            <i class="bx bxs-time"></i>
                            <span>${movie.durationInHours * 60} mins</span>
                        </div>
                        <div class="movie-info">
                            <span>${movie.genre}</span>
                        </div>
                        <div class="movie-info">
                            <span>${movie.certification}</span>
                        </div>
                    </div>
                    <div class="item-content-description top-down delay-4">
                        ${movie.description}
                    </div>
                    <div class="item-action top-down delay-6">
                        <a href="movie.html?title=${encodeURIComponent(
                          movie.title
                        )}" class="btn btn-hover">
                            <i class="bx bxs-right-arrow"></i>
                            <span>Book Now</span>
                        </a>
                    </div>
                </div>
            </div>
    `;
    return heroslideitem;
  }
  const fetchmovies = () => {
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
        const moviesContainer = document.querySelector(".running-movies");

        const heroCarousel = document.getElementById("hero-carousel");

        data.forEach((movie) => {
          const slideItem = createSlideItem(movie);
          heroCarousel.append(slideItem);
        });

        $("#hero-carousel").owlCarousel({
          items: 1,
          dots: false,
          loop: true,
          nav: true,
          navText: navText,
          autoplay: true,
          autoplayHoverPause: true,
        });

        data.forEach((movie) => {
          const movieItem = document.createElement("a");
          movieItem.href = `movie.html?title=${encodeURIComponent(
            movie.title
          )}`;
          movieItem.className = "movie-item";

          const img = document.createElement("img");
          img.src = movie.mainImage; // Assuming mainImage is the correct property
          img.alt = ""; // Set appropriate alt text

          const movieItemContent = document.createElement("div");
          movieItemContent.className = "movie-item-content";

          const movieTitle = document.createElement("div");
          movieTitle.className = "movie-item-title";
          movieTitle.textContent = movie.title;

          const movieInfos = document.createElement("div");
          movieInfos.className = "movie-infos";

          // Example movie info: Rating
          const ratingInfo = document.createElement("div");
          ratingInfo.className = "movie-info";
          const ratingIcon = document.createElement("i");
          ratingIcon.className = "bx bxs-star";
          const ratingValue = document.createElement("span");
          ratingValue.textContent = movie.averageRating.toFixed(1); // Assuming averageRating is available
          ratingInfo.appendChild(ratingIcon);
          ratingInfo.appendChild(ratingValue);

          // Example movie info: Duration
          const durationInfo = document.createElement("div");
          durationInfo.className = "movie-info";
          const durationIcon = document.createElement("i");
          durationIcon.className = "bx bxs-time";
          const durationValue = document.createElement("span");
          durationValue.textContent = `${movie.durationInHours * 60} mins`; // Convert hours to minutes
          durationInfo.appendChild(durationIcon);
          durationInfo.appendChild(durationValue);

          const qualityInfo = document.createElement("div");
          qualityInfo.className = "movie-info";
          const qualityValue = document.createElement("span");
          qualityValue.textContent = `HD`;
          qualityInfo.appendChild(qualityValue);

          const certificateInfo = document.createElement("div");
          certificateInfo.className = "movie-info";
          const certificateValue = document.createElement("span");
          certificateValue.textContent = `${movie.certification}`;
          certificateInfo.appendChild(certificateValue);

          // Append elements together
          movieInfos.appendChild(ratingInfo);
          movieInfos.appendChild(durationInfo);
          movieInfos.appendChild(qualityInfo);
          movieInfos.appendChild(certificateInfo);

          movieItemContent.appendChild(movieTitle);
          movieItemContent.appendChild(movieInfos);

          movieItem.appendChild(img);
          movieItem.appendChild(movieItemContent);
          moviesContainer.appendChild(movieItem);
        });

        var $owl = $(".movies-slide").owlCarousel({
          items: 2,
          dots: false,
          nav: true,
          navText: navText,
          margin: 15,
          responsive: {
            500: {
              items: 2,
            },
            1280: {
              items: 4,
            },
            1600: {
              items: 6,
            },
          },
        });
      })
      .catch((error) => {
        console.error("Error fetching movies:", error);
      });
  };

  fetchmovies();

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

  if (localStorage.getItem("token")) {
    document.getElementById("logged-in").innerHTML = "Log Out";
  }

  document.getElementById("logged-in").addEventListener("click", () => {
    if (document.getElementById("logged-in").innerHTML == "Log Out") {
      document.getElementById("logged-in").innerHTML = "Sign In";
      window.localStorage.removeItem("token");
    }
  });

  let navText = [
    "<i class='bx bx-chevron-left'></i>",
    "<i class='bx bx-chevron-right'></i>",
  ];

  $("#top-movies-slide").owlCarousel({
    items: 2,
    dots: false,
    loop: true,
    autoplay: true,
    autoplayHoverPause: true,
    responsive: {
      500: {
        items: 3,
      },
      1280: {
        items: 4,
      },
      1600: {
        items: 6,
      },
    },
  });
});
