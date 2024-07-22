document.addEventListener("DOMContentLoaded", () => {
  const loader = document.getElementById("loader");

  const showLoader = () => {
    loader.classList.add("active");
  };

  document.querySelector(".ad-nav-content").addEventListener("click", () => {
    window.location.href = "booking_orders.html";
  });

  // Function to hide loader
  const hideLoader = () => {
    loader.classList.remove("active");
  };
  var selectedMethod = "Credit Card";
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
        document.querySelector(".bo-container").style.opacity = "1.0";
      }
    }
  });

  var locationdown = document.querySelector(".down-button");
  locationdown.addEventListener("click", () => {
    var popuplocation = document.querySelector(".popup-location");
    if (popuplocation.classList.contains("active")) {
      document.querySelector(".bo-container").style.opacity = "1.0";
      popuplocation.classList.remove("active");
    } else {
      document.querySelector(".bo-container").style.opacity = "0.3";
      popuplocation.classList.add("active");
    }
  });

  const cityButtons = document.querySelectorAll(".pcities button");
  const locationSpan = document.querySelector(".location span");

  cityButtons.forEach((button) => {
    button.addEventListener("click", () => {
      var popuplocation = document.querySelector(".popup-location");
      document.querySelector(".bo-container").style.opacity = "1.0";
      popuplocation.classList.remove("active");
      locationSpan.innerHTML = button.innerHTML;
    });
  });

  const fetchAllUserBookings = async () => {
    try {
      showLoader();

      const response = await fetch(
        `http://localhost:5091/api/Booking/GetAllUserBookings`,
        {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );

      if (!response.ok) {
        throw new Error(`HTTP error! status: ${response.status}`);
      }

      const data = await response.json();
      console.log(data);
      displayBookings(data);
      hideLoader();
    } catch (err) {
      console.error("Error fetching user bookings:", err);
    } finally {
      hideLoader();
    }
  };

  //   public int BookingId { get; set; }
  // public string TheaterName { get; set; }
  // public string MovieName { get; set; }
  // public string ScreenName { get; set; }
  // public List<string> BookedTickets { get; set; }
  // public Dictionary<string, int>? SnacksOrderedWithTickets { get; set; }
  // public decimal TotalPrice { get; set; }
  // public string PaymentStatus { get; set; }

  const displayBookings = (bookings) => {
    const ordersList = document.querySelector(".bo-container");
    ordersList.innerHTML = ""; // Clear previous bookings
    const currentTime = new Date();

    bookings.forEach((booking) => {
      const ticket = document.createElement("div");
      ticket.classList.add("ticket");
      const snacksfound = booking.snacksOrderedWithTickets;
      const showtime = new Date(booking.showtime);
      const timeDifference = showtime - currentTime;
      const hoursDifference = timeDifference / (1000 * 60 * 60);
      const minutesDifference = timeDifference / (1000 * 60);

      let cancellationReason = "";
      let showCancelButton = false;

      if (hoursDifference >= 2) {
        showCancelButton = true;
      } else if (minutesDifference <= 0) {
        cancellationReason = "Cancellation is not allowed for these tickets";
      } else if (minutesDifference <= 10) {
        cancellationReason =
          "Cancellation is not allowed for before 10mins of the show";
      } else if (showtime <= currentTime) {
        cancellationReason =
          "The show has started. Cancellation is not allowed.";
      } else if (hoursDifference < 2) {
        cancellationReason =
          "Cancellation is only allowed at least 2 hours before the showtime.";
      }
      ticket.innerHTML = `
        <div class="movie-details-1">
          <h1 id="movie-name">${booking.movieName}</h1>
          <div class="tikets-div">
            <span id="noofticket">${booking.bookedTickets.length}</span>
            <span>Tickets</span>
          </div>
        </div>
        <div class="movie-details-2">
          <div class="theater-screen">
            <span id="theater-name">${booking.theaterName}</span>
            <span id="screen-name">(${booking.screenName})</span>
          </div>
        </div>
        <div class="movie-details-3">
          <span id="showtime">${showtime.toLocaleString()}</span>
          <span id="booked-tickets">${booking.bookedTickets.join(",")}</span>
          <div id="booked-snacks">${Object.keys(
            booking.snacksOrderedWithTickets
          ).map((snack) => {
            return `
            <span>${snack}<span class="confirmed">(${snacksfound[snack]})</span></span>
            `;
          })}</div>
        </div>
        <div class="movie-details-4">
          <div class="status">
            <span id="booking-status">Booking : <span class="confirmed">${
              booking.paymentStatus
            }</span></span>
            <span id="payment-status">Payment : <span class="confirmed">${
              booking.paymentStatus
            }</span></span>
          </div>
          <div class="payment-amount">
            <span>Total Amount <span id="amount">₹${
              booking.totalPrice
            }</span></span>
          </div>
        </div>
              ${
                showCancelButton
                  ? `<button class="cancel-button" data-booking-id="${booking.bookingId}">Cancel</button>`
                  : `<span class="cancellation-reason pending">${cancellationReason}</span>`
              }

      `;

      ordersList.appendChild(ticket);
    });
    addTicketActionListeners();
  };

  fetchAllUserBookings();
  const addTicketActionListeners = () => {
    document.querySelectorAll(".cancel-button").forEach((button) => {
      button.addEventListener("click", () => {
        const bookingId = button.getAttribute("data-booking-id");
        window.location.href = `cancellationticket.html?bookingId=${bookingId}`;
      });
    });
  };
});
