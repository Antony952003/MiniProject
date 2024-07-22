document.addEventListener("DOMContentLoaded", () => {
  const loader = document.getElementById("loader");
  const mainContainer = document.querySelector(".main-container");

  // Function to show loader
  const showLoader = () => {
    loader.classList.add("active");
    mainContainer.style.opacity = 0;
  };

  // Function to hide loader
  const hideLoader = () => {
    loader.classList.remove("active");
    mainContainer.style.opacity = 1;
  };
  document.querySelector(".logo").addEventListener("click", () => {
    window.location.href = "index.html";
  });
  document.querySelector(".ad-nav-content").addEventListener("click", () => {
    window.location.href = "booking_orders.html";
  });
  const addratenoweventlistener = () => {
    document.getElementById("rate-now").addEventListener("click", function () {
      console.log(document.getElementById("rate-now"));
      document.getElementById("popup").classList.add("active");
    });
  };
  var movieid = 0;
  async function fetchMovieData() {
    try {
      const params = new URLSearchParams(window.location.search);
      const movieTitle = params.get("title");
      const response = await fetch(
        `http://localhost:5091/api/Movie/GetMovieByName?name=${movieTitle}`,
        {
          method: "GET",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      const data = await response.json();
      console.log(data);

      // Update DOM with fetched data
      const mainContainer = document.querySelector(".main-container");
      movieid = data.movieId;
      function convertHoursToHoursMinutes(durationInHours) {
        // Extract whole hours and minutes
        const hours = Math.floor(durationInHours);
        const minutes = Math.round((durationInHours - hours) * 60);

        // Format the result
        const formattedDuration = `${hours}hr ${minutes}mins`;

        return formattedDuration;
      }
      function formatDate(dateString) {
        const date = new Date(dateString);

        // Define month names
        const monthNames = [
          "January",
          "February",
          "March",
          "April",
          "May",
          "June",
          "July",
          "August",
          "September",
          "October",
          "November",
          "December",
        ];

        // Get day with ordinal suffix
        const day = date.getDate();
        let dayWithSuffix;
        if (day === 1 || day === 21 || day === 31) {
          dayWithSuffix = day + "st";
        } else if (day === 2 || day === 22) {
          dayWithSuffix = day + "nd";
        } else if (day === 3 || day === 23) {
          dayWithSuffix = day + "rd";
        } else {
          dayWithSuffix = day + "th";
        }

        // Get month and year
        const month = monthNames[date.getMonth()];
        const year = date.getFullYear();

        // Construct formatted date string
        const formattedDate = `${dayWithSuffix} ${month} ${year}`;

        return formattedDate;
      }

      mainContainer.innerHTML = `
         <section class="hero-container">
        <div class="hero-subcontainer">
          <div class="hero-elements">
            <div class="hero-image">
              <img
                src="${data.mainImage}"
                alt="${data.title}"
                width="100%"
                height="100%"
              />
              <div class="incinemas">In Cinemas</div>
            </div>
            <div class="hero-details">
              <h1>${data.title}</h1>
              <div class="hero-rating">
                <div class="hero-rating-left">
                  <span class="star">
                    <i class="bx bxs-star"></i>
                  </span>
                  <span class="imdb-rating"> ${data.averageRating}/10 </span>
                  <span class="votes"> (25.1k votes) <span>></span> </span>
                </div>
                <button id="rate-now">Rate Now</button>
              </div>
              <div class="hero-format-details">
                <div class="movie-formats">
                  <a href="">2D</a>, <a href="">2D SCREEN X</a>,
                  <a href="">mx4d</a>, <a href="">4dx</a>,
                  <a href="">Imax 2d</a>,
                  <a href="">ice</a>
                </div>
                <div class="movie-languages">
                  <a href="">Tamil</a>, <a href="">English</a>,
                  <a href="">Telugu</a>, <a href="">Malayalam</a>,
                  <a href="">Kannada</a>,
                  <a href="">Hindi</a>
                </div>
              </div>
              <div class="movie-details">
                <span class="time"> ${convertHoursToHoursMinutes(
                  data.durationInHours
                )} </span>
                <ul>
                  <li>${data.genre}</li>
                  <li>${data.certification}</li>
                  <li>${formatDate(data.releaseDate)}</li>
                </ul>
              </div>
              <div class="booknow">
                <a href="showtimelisting.html?moviename=${encodeURIComponent(
                  data.title
                )}" class="btn btn-hover">
                  <span>Book Now</span>
                </a>
              </div>
            </div>
          </div>
        </div>
      </section>
      <!-- End Hero Container -->
      <section class="synopsis-container">
        <div class="synopsis">
          <div class="component-1">
            <h2 class="about-1">About the movie</h2>
            <p class="about-2">
              ${data.description}
            </p>
          </div>
          <!-- Cast -->
          <div class="component-2">
            <h2>Cast</h2>
            <div class="cast-container swiper-container">
              <div class="swiper-wrapper">
              ${data.castMemberRoles
                .map((castmember) => {
                  return `
                  <div class="cast-card swiper-slide">
                  <img
                    alt="${castmember.artistName}"
                    src="${castmember.artistImage}"
                  />
                  <div class="cast-info">
                    <p>${castmember.artistName}</p>
                    <span>as ${castmember.characterName}</span>
                  </div>
                </div>
                  `;
                })
                .join("")}
              </div>
            </div>
          </div>
          <!-- End Cast Container -->
          <!-- Reviews -->
          <!-- End Of Reviews -->
        </div>
      </section>
      `;

      const rateNowButton = mainContainer.querySelector("#rate-now");
      rateNowButton.addEventListener("click", function () {
        document.getElementById("popup").classList.add("active");
        mainContainer.style.opacity = 0.2;
      });

      const herosubcontainer = document.querySelector(".hero-subcontainer");
      function setBackgroundImage() {
        if (window.innerWidth >= 810) {
          herosubcontainer.style.backgroundImage = `linear-gradient(
            90deg,
            rgb(26, 26, 26) 24.97%,
            rgb(26, 26, 26) 38.3%,
            rgba(26, 26, 26, 0.04) 97.47%,
            rgb(26, 26, 26) 100%
          ),
          url('${data.bannerImage}')`;
        } else {
          herosubcontainer.style.backgroundImage = "none";
        }
      }
      setBackgroundImage();
      fetchReviewsData(data.title);

      var swiper = new Swiper(".swiper-container", {
        slidesPerView: 1,
        spaceBetween: 10,
        navigation: false,
        pagination: false,
        breakpoints: {
          640: {
            slidesPerView: 2,
            spaceBetween: 10,
          },
          768: {
            slidesPerView: 3,
            spaceBetween: 20,
          },
          1024: {
            slidesPerView: 4,
            spaceBetween: 30,
          },
        },
      });

      var swiper2 = new Swiper(".hashtag-container", {
        slidesPerView: 1,
        spaceBetween: 10,
        navigation: false,
        pagination: false,
        breakpoints: {
          640: {
            slidesPerView: 2,
            spaceBetween: 10,
          },
          768: {
            slidesPerView: 2,
            spaceBetween: 20,
          },
          1024: {
            slidesPerView: 2,
            spaceBetween: 30,
          },
        },
      });

      window.addEventListener("resize", setBackgroundImage);

      // const leftBtn = document.querySelector(".left-btn-content");
      // const rightBtn = document.querySelector(".right-btn-content");
      // const reviewContainer = document.querySelector(".review-card-container");
      // const slideWidth = 446; // Width of each slide

      // let currentTranslateX = 0;

      // rightBtn.addEventListener("click", function () {
      //   const maxTranslateX =
      //     reviewContainer.scrollWidth - reviewContainer.clientWidth;
      //   if (currentTranslateX < maxTranslateX) {
      //     currentTranslateX += slideWidth;
      //     reviewContainer.style.transform = `translateX(-${currentTranslateX}px)`;
      //   }
      // });

      // leftBtn.addEventListener("click", function () {
      //   if (currentTranslateX > 0) {
      //     currentTranslateX -= slideWidth;
      //     reviewContainer.style.transform = `translateX(-${currentTranslateX}px)`;
      //   }
      // });
    } catch (error) {
      console.error("Error fetching movie data:", error);
    }
  }
  showLoader();
  fetchMovieData();
  hideLoader();

  const fetchReviewsData = (moviename) => {
    fetch(
      `http://localhost:5091/api/Review/GetMovieReviews?moviename=${moviename}
`,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    )
      .then((res) => res.json())
      .then((data) => {
        console.log(data);
        console.log(data[0].hashTags);
        const component3div = document.createElement("div");
        component3div.classList.add("component-3");
        component3div.innerHTML = `
            <div class="review-wrapper">
              <div class="review-head">
                <h2>Top Reviews</h2>
                <span>11.3k reviews <a href="#">></a></span>
              </div>

              <div class="summary-reviews">
                <p>Summmary of 11.3k reviews.</p>
                <div class="hashtag-container swiper-container">
                  <div class="swiper-wrapper">
                  ${data
                    .map((review) =>
                      review.hashTags
                        .split(",")
                        .map(
                          (hashtag) => `
                    <div class="hashtag-card swiper-slide">
                      <span>${hashtag}<span class="hashtag-count">23</span></span>
                    </div>
                  `
                        )
                        .join("")
                    )
                    .join("")}
                  </div>
                </div>
              </div>

              <div class="top-review-wrapper">
                <div class="review-card-container">
                ${data
                  .map(
                    (review) => `
                  <div class="review-card">
                    <div class="review-profile">
                      <div class="user-profile">
                        ${
                          review.userImage == "No Image"
                            ? `<i class="bx bxs-user-circle"></i>
                        `
                            : `<img class="review-image" src="${review.userImage}" alt="" />`
                        }
                        <span>${review.userName}</span>
                      </div>
                      <div class="review-rating">
                        <i class="bx bxs-star"></i>
                        <span>${review.rating}/10</span>
                      </div>
                    </div>
                    <h2>${review.hashTags
                      .split(",")
                      .map((hashtag) => {
                        return `${hashtag}`;
                      })
                      .join("")}</h2>
                    <p class="review-description">
                      ${review.comment}
                    </p>
                    <span class="read-more">more...</span>
                    <div class="card-footer">
                      <div class="opinion">
                        <div class="like">
                          <i class="bx bx-like"></i>
                          <span class="like-count">${review.likes}</span>
                        </div>
                        <div class="dislike">
                          <i class="bx bx-dislike"></i>
                          <span class="dislike-count">${review.disLikes}</span>
                        </div>
                      </div>
                      <div class="date-share">
                        <span class="posted-date">${timeAgo(
                          review.createdAt
                        )}</span>
                        <i class="bx bx-share-alt"></i>
                      </div>
                    </div>
                  </div>
                `
                  )
                  .join("")}
                  <div class="left-button"></div>
                  <div class="right-button"></div>
                </div>
                <div class="left-btn">
                  <div class="left-btn-content">
                    <i class="bx bx-chevron-left"></i>
                  </div>
                </div>
                <div class="right-btn">
                  <div class="right-btn-content">
                    <i class="bx bx-chevron-right"></i>
                  </div>
                </div>
              </div>
            </div>
        `;
        function timeAgo(timestamp) {
          const now = new Date();
          const then = new Date(timestamp);
          const diffInSeconds = Math.floor((now - then) / 1000);

          if (diffInSeconds < 60) {
            return `just now`;
          }

          const minutes = Math.floor(diffInSeconds / 60);
          const hours = Math.floor(minutes / 60);
          const days = Math.floor(hours / 24);
          const weeks = Math.floor(days / 7);
          const months = Math.floor(days / 30); // Approximation
          const years = Math.floor(days / 365); // Approximation

          if (years > 0) {
            return `${years} year${years > 1 ? "s" : ""} ago`;
          } else if (months > 0) {
            return `${months} month${months > 1 ? "s" : ""} ago`;
          } else if (weeks > 0) {
            return `${weeks} week${weeks > 1 ? "s" : ""} ago`;
          } else if (days > 0) {
            return `${days} day${days > 1 ? "s" : ""} ago`;
          } else if (hours > 0) {
            return `${hours} hour${hours > 1 ? "s" : ""} ago`;
          } else {
            return `${minutes} minute${minutes > 1 ? "s" : ""} ago`;
          }
        }

        console.log(timeAgo("2024-07-15T10:00:00.0000000Z"));

        const synopsis = document.querySelector(".synopsis");
        synopsis.appendChild(component3div);
        var swiper = new Swiper(".swiper-container", {
          slidesPerView: 1,
          spaceBetween: 10,
          navigation: false,
          pagination: false,
          breakpoints: {
            640: {
              slidesPerView: 2,
              spaceBetween: 10,
            },
            768: {
              slidesPerView: 3,
              spaceBetween: 20,
            },
            1024: {
              slidesPerView: 4,
              spaceBetween: 30,
            },
          },
        });

        var swiper2 = new Swiper(".hashtag-container", {
          slidesPerView: 1,
          spaceBetween: 10,
          navigation: false,
          pagination: false,
          breakpoints: {
            640: {
              slidesPerView: 2,
              spaceBetween: 10,
            },
            768: {
              slidesPerView: 2,
              spaceBetween: 20,
            },
            1024: {
              slidesPerView: 2,
              spaceBetween: 30,
            },
          },
        });

        const leftBtn = document.querySelector(".left-btn-content");
        const rightBtn = document.querySelector(".right-btn-content");
        const reviewContainer = document.querySelector(
          ".review-card-container"
        );
        const slideWidth = 446; // Width of each slide

        let currentTranslateX = 0;

        rightBtn.addEventListener("click", function () {
          const maxTranslateX =
            reviewContainer.scrollWidth - reviewContainer.clientWidth;
          if (currentTranslateX < maxTranslateX) {
            currentTranslateX += slideWidth;
            reviewContainer.style.transform = `translateX(-${currentTranslateX}px)`;
          }
        });

        leftBtn.addEventListener("click", function () {
          if (currentTranslateX > 0) {
            currentTranslateX -= slideWidth;
            reviewContainer.style.transform = `translateX(-${currentTranslateX}px)`;
          }
        });
      });
  };

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

  if (localStorage.getItem("token")) {
    document.getElementById("logged-in").innerHTML = "Log Out";
  }

  document.getElementById("logged-in").addEventListener("click", () => {
    if (document.getElementById("logged-in").innerHTML == "Log Out") {
      document.getElementById("logged-in").innerHTML = "Sign In";
      window.localStorage.removeItem("token");
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

  document
    .getElementById("rateStars")
    .addEventListener("mouseover", function (event) {
      if (event.target.classList.contains("bx-star")) {
        const stars = document.querySelectorAll(".rate-stars i");
        const rating = event.target.dataset.value;
        console.log(rating);

        stars.forEach((star) => {
          if (parseInt(star.dataset.value) <= parseInt(rating)) {
            console.log(star.dataset.value);
            star.classList.toggle("selected");
            star.classList.toggle("bxs-star");
          }
        });
      }
    });

  // Event listener for clicking stars
  document
    .getElementById("rateStars")
    .addEventListener("click", function (event) {
      if (event.target.classList.contains("bx-star")) {
        const stars = document.querySelectorAll(".rate-stars i");
        const rating = event.target.dataset.value;

        stars.forEach((star) => {
          star.classList.remove("selected");
          star.classList.remove("bxs-star");
          if (parseInt(star.dataset.value) <= parseInt(rating)) {
            console.log(star.dataset.value);
            star.classList.toggle("selected");
            star.classList.add("bxs-star");
          }
        });
      }
    });

  // Event listener for adding hashtags
  document.getElementById("addHashtag").addEventListener("click", function () {
    const hashtagInput = document.getElementById("hashtagInput");
    const hashtagList = document.getElementById("hashtagList");
    const hashtagText = hashtagInput.value.trim();

    if (hashtagText) {
      const hashtagDiv = document.createElement("div");
      hashtagDiv.className = "hashtag";
      hashtagDiv.textContent = hashtagText;
      hashtagList.appendChild(hashtagDiv);
      hashtagInput.value = "";
    }
  });

  // Event listener for submitting review
  document
    .getElementById("review-submit")
    .addEventListener("click", function (event) {
      event.preventDefault();

      const rating = document.querySelectorAll(".rate-stars i.selected");
      var ratingmax = 0;
      rating.forEach((star) => {
        if (parseInt(star.dataset.value) > ratingmax) {
          ratingmax = parseInt(star.dataset.value);
        }
      });
      const comment = document.getElementById("comment").value;
      const hashtags = Array.from(document.querySelectorAll(".hashtag")).map(
        (tag) => tag.textContent
      );

      // Handle the form submission logic here
      console.log("Rating:", ratingmax);
      console.log("Comment:", comment);
      console.log("Hashtags:", hashtags.join(","));

      var postreviewdata = {
        movieId: movieid,
        comment: comment,
        rating: ratingmax,
        hashTags: hashtags.join(","),
      };

      fetch(`http://localhost:5091/api/Review/GiveReview`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify(postreviewdata),
      }).then((res) =>
        res.json().then((data) => {
          console.log(data);
          setTimeout(() => {
            Toastify({
              text: `Review added Successfully!!`,
              duration: 3000,
              newWindow: true,
              close: true,
              gravity: "top", // `top` or `bottom`
              position: "right", // `left`, `center` or `right`
              stopOnFocus: true, // Prevents dismissing of toast on hover
              style: {
                background:
                  "linear-gradient(to right, #BDFF6C, #A7FF3B,#8FFE09)",
                color: "black",
              },
              onClick: function () {}, // Callback after click
            }).showToast();
          }, 6000);
        })
      );

      document.getElementById("popup").classList.remove("active");
      document.querySelector(".main-container").style.opacity = 1.0;
    });
});
