document.addEventListener("DOMContentLoaded", () => {
  // Redirect to index.html on logo click
  document.querySelector(".logo").addEventListener("click", () => {
    window.location.href = "index.html";
  });
  document.querySelector(".ad-nav-content").addEventListener("click", () => {
    window.location.href = "booking_orders.html";
  });

  // Function to fetch user details
  const fetchUserDetails = () => {
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
        var imageEles = document.querySelectorAll(".user-profile-image");
        imageEles.forEach((imageEle) => {
          imageEle.src = data.image;
          imageEle.onerror = function () {
            imageEle.src = "../Assets/Images/user_image.png";
          };
        });
      })
      .catch((error) => {
        console.error("Error fetching user details:", error);
      });
  };
  fetchUserDetails();

  // Toggle user menu on click
  document.querySelector(".user").addEventListener("click", () => {
    const userMenu = document.querySelector(".account-details-nav");
    const navMenu = document.querySelector("#nav-menu");
    const hamburgerMenu = document.querySelector("#hamburger-menu");

    if (!document.querySelector(".user").classList.contains("active")) {
      document.querySelector(".user").classList.add("active");
      userMenu.classList.add("active");

      if (navMenu.classList.contains("active")) {
        navMenu.classList.remove("active");
        hamburgerMenu.classList.remove("active");
      }
    } else {
      document.querySelector(".user").classList.remove("active");
      userMenu.classList.remove("active");
    }
  });

  // Toggle user menu from alternate navigation header
  document
    .querySelector(".ad-nav-header-comp-2")
    .addEventListener("click", () => {
      const userMenu = document.querySelector(".account-details-nav");
      const navMenu = document.querySelector("#nav-menu");
      const hamburgerMenu = document.querySelector("#hamburger-menu");

      if (!document.querySelector(".user").classList.contains("active")) {
        document.querySelector(".user").classList.add("active");
        userMenu.classList.add("active");

        if (navMenu.classList.contains("active")) {
          navMenu.classList.remove("active");
          hamburgerMenu.classList.remove("active");
        }
      } else {
        document.querySelector(".user").classList.remove("active");
        userMenu.classList.remove("active");
      }
    });

  // Check if user is logged in and update UI accordingly
  if (localStorage.getItem("token")) {
    document.getElementById("logged-in").innerHTML = "Log Out";
  }

  // Log out user on click
  document.getElementById("logged-in").addEventListener("click", () => {
    if (document.getElementById("logged-in").innerHTML === "Log Out") {
      document.getElementById("logged-in").innerHTML = "Sign In";
      window.localStorage.removeItem("token");
    }
  });

  // Toggle hamburger menu on click
  document.querySelector(".hamburger-menu").addEventListener("click", () => {
    const hamburgerMenu = document.querySelector(".hamburger-menu");
    const navMenu = document.querySelector(".nav-menu");
    const popupLocation = document.querySelector(".popup-location");
    const showtimeContainer = document.querySelector(".showtimecontainer");

    if (!hamburgerMenu.classList.contains("active")) {
      hamburgerMenu.classList.add("active");
      navMenu.classList.add("active");
    } else {
      hamburgerMenu.classList.remove("active");
      navMenu.classList.remove("active");

      if (popupLocation.classList.contains("active")) {
        popupLocation.classList.remove("active");
        showtimeContainer.style.opacity = "1.0";
      }
    }
  });

  // Toggle location popup visibility
  document.querySelector(".down-button").addEventListener("click", () => {
    const popupLocation = document.querySelector(".popup-location");
    const showtimeContainer = document.querySelector(".showtimecontainer");

    if (popupLocation.classList.contains("active")) {
      showtimeContainer.style.opacity = "1.0";
      popupLocation.classList.remove("active");
    } else {
      showtimeContainer.style.opacity = "0.3";
      popupLocation.classList.add("active");
    }
  });

  // Handle city button click to update location span
  const cityButtons = document.querySelectorAll(".pcities button");
  const locationSpan = document.querySelector(".location span");

  cityButtons.forEach((button) => {
    button.addEventListener("click", () => {
      const popupLocation = document.querySelector(".popup-location");
      const showtimeContainer = document.querySelector(".showtimecontainer");

      showtimeContainer.style.opacity = "1.0";
      popupLocation.classList.remove("active");
      locationSpan.innerHTML = button.innerHTML;
    });
  });

  // Get movie title from URL parameter
  const movieTitle = new URLSearchParams(window.location.search).get(
    "moviename"
  );
  document.getElementById("movie-title").innerText = movieTitle;
  const genreSpans = document.querySelector(".genre-spans");

  const fetchmoviedetails = () => {
    fetch(`http://localhost:5091/api/Movie/GetMovieByName?name=${movieTitle}`, {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    })
      .then((res) => res.json())
      .then((data) => {
        console.log(data);
        genrelist = data.genre.split(",");
        genrelist.forEach((genre) => {
          var spanele = document.createElement("span");
          spanele.innerHTML = genre;
          genreSpans.appendChild(spanele);
        });
      });
  };
  fetchmoviedetails();

  // Number of days to show for day buttons
  const daysOfWeek = 7;
  const today = new Date();
  const dayButtonsContainer = document.getElementById("day-buttons");

  // Fetch and display showtimes based on filters
  const fetchShowtimes = () => {
    const activeDayButton = document.querySelector(".day-button.active");
    const selectedDate = activeDayButton ? activeDayButton.dataset.date : null;
    const formatFilter = document.getElementById("format-filter").value;
    const timeFilter = document.getElementById("time-filter").value;

    fetch(
      `http://localhost:5091/api/Showtime/GetAllShowtimesofMovie?moviename=${encodeURIComponent(
        movieTitle
      )}`,
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
      }
    )
      .then((response) => response.json())
      .then((data) => {
        console.log("Fetched showtimes:", data);

        const theaters = data.reduce((acc, showtime) => {
          const showtimeDate = showtime.startTime.split("T")[0];

          // Only include showtimes for the selected date
          if (showtimeDate === selectedDate) {
            const theaterExists = acc.find(
              (theater) => theater.name === showtime.theaterName
            );
            if (theaterExists) {
              theaterExists.showtimes.push({
                time: new Date(showtime.startTime).toLocaleTimeString([], {
                  hour: "2-digit",
                  minute: "2-digit",
                }),
                format: showtime.screenType, // Use screenType as format
                showtimeId: showtime.showtimeId, // Add showtimeId here
              });
            } else {
              acc.push({
                name: showtime.theaterName,
                showtimes: [
                  {
                    time: new Date(showtime.startTime).toLocaleTimeString([], {
                      hour: "2-digit",
                      minute: "2-digit",
                    }),
                    format: showtime.screenType, // Use screenType as format
                    showtimeId: showtime.showtimeId, // Add showtimeId here
                  },
                ],
              });
            }
          }
          return acc;
        }, []);

        // Apply format and time filters after filtering by date
        const filteredTheaters = theaters
          .map((theater) => {
            return {
              ...theater,
              showtimes: theater.showtimes.filter((showtime) => {
                const showtimeFormatMatches =
                  !formatFilter || showtime.format === formatFilter;
                const showtimeTimeMatches =
                  !timeFilter ||
                  (timeFilter === "morning" &&
                    showtime.time >= "08:00 AM" &&
                    showtime.time < "12:00 PM") ||
                  (timeFilter === "afternoon" &&
                    showtime.time >= "12:00 PM" &&
                    showtime.time < "04:00 PM") ||
                  (timeFilter === "evening" &&
                    showtime.time >= "04:00 PM" &&
                    showtime.time < "08:00 PM") ||
                  (timeFilter === "night" &&
                    showtime.time >= "08:00 PM" &&
                    showtime.time <= "12:00 AM");

                return showtimeFormatMatches && showtimeTimeMatches;
              }),
            };
          })
          .filter((theater) => theater.showtimes.length > 0);

        const theaterListContainer = document.getElementById("theater-list");
        console.log(filteredTheaters);
        theaterListContainer.innerHTML = filteredTheaters
          .map(
            (theater, index) =>
              `
          <div class="theater delay-${index + 1}">
            <h2>${theater.name}</h2>
            <div class="showtimes">
              ${theater.showtimes
                .map(
                  (showtime) => `
                  <div class="showtime">
                    <a href="seat.html?showtimeid=${encodeURIComponent(
                      showtime.showtimeId
                    )}">
                    ${showtime.time} (${showtime.format})</a>
                  </div>
                `
                )
                .join("")}
            </div>
          </div>
        `
          )
          .join("");
      })
      .catch((error) => {
        console.error("Error fetching showtimes:", error);
      });
  };

  // Create day buttons and handle click events
  for (let i = 0; i < daysOfWeek; i++) {
    const date = new Date(today);
    date.setDate(today.getDate() + i);

    const dayButton = document.createElement("button");
    dayButton.classList.add("day-button");
    const dayspan = document.createElement("span");
    dayspan.innerHTML = date.toDateString().slice(0, 3);
    const datespan = document.createElement("span");
    datespan.innerHTML = date.toDateString().slice(8, 11);
    const monthspan = document.createElement("span");
    monthspan.innerHTML = date.toDateString().slice(4, 7);
    dayspan.classList.add("dayspan");
    datespan.classList.add("datespan");
    monthspan.classList.add("monthspan");
    dayButton.appendChild(dayspan);
    dayButton.appendChild(datespan);
    dayButton.appendChild(monthspan);

    // console.log(date.toDateString());
    // dayButton.innerText =
    //   date.toDateString().slice(0, 3) +
    //   " " +
    //   date.toDateString().slice(8, 11) +
    //   " " +
    //   date.toDateString().slice(4, 7);
    dayButton.dataset.date = date.toISOString().split("T")[0];

    if (i === 0) {
      dayButton.classList.add("active");
    }

    dayButton.addEventListener("click", () => {
      document
        .querySelectorAll(".day-button")
        .forEach((button) => button.classList.remove("active"));
      dayButton.classList.add("active");
      fetchShowtimes();
    });
    dayButtonsContainer.appendChild(dayButton);
  }

  document
    .getElementById("format-filter")
    .addEventListener("change", fetchShowtimes);
  document
    .getElementById("time-filter")
    .addEventListener("change", fetchShowtimes);

  fetchShowtimes();
});
