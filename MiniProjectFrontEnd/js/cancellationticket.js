document.addEventListener("DOMContentLoaded", () => {
  const loader = document.getElementById("loader");

  const showLoader = () => {
    loader.classList.add("active");
    document.querySelector(".ct-container").style.display = "none";
  };

  document.querySelector(".ad-nav-content").addEventListener("click", () => {
    window.location.href = "booking_orders.html";
  });

  // Function to hide loader
  const hideLoader = () => {
    loader.classList.remove("active");
    document.querySelector(".ct-container").style.display = "flex";
  };
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
        document.querySelector(".ct-container").style.opacity = "1.0";
      }
    }
  });

  var locationdown = document.querySelector(".down-button");
  locationdown.addEventListener("click", () => {
    var popuplocation = document.querySelector(".popup-location");
    if (popuplocation.classList.contains("active")) {
      document.querySelector(".ct-container").style.opacity = "1.0";
      popuplocation.classList.remove("active");
    } else {
      document.querySelector(".ct-container").style.opacity = "0.3";
      popuplocation.classList.add("active");
    }
  });

  const cityButtons = document.querySelectorAll(".pcities button");
  const locationSpan = document.querySelector(".location span");

  cityButtons.forEach((button) => {
    button.addEventListener("click", () => {
      var popuplocation = document.querySelector(".popup-location");
      document.querySelector(".ct-container").style.opacity = "1.0";
      popuplocation.classList.remove("active");
      locationSpan.innerHTML = button.innerHTML;
    });
  });
  const bookingId = new URLSearchParams(window.location.search).get(
    "bookingId"
  );

  const fetchBookingDetails = async (bookingId) => {
    try {
      showLoader();
      const response = await fetch(
        `http://localhost:5091/api/Booking/GetBookingById?bookingId=${bookingId}`,
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

      const booking = await response.json();
      hideLoader();
      console.log(booking);
      displayBookingDetails(booking);
    } catch (err) {
      console.error("Error fetching booking details:", err);
    }
  };

  const displayBookingDetails = (booking) => {
    const bookingDetailsDiv = document.getElementById("booking-details");
    bookingDetailsDiv.innerHTML = `
          <h1>${booking.movieName}</h1>
          <p>Theater: ${booking.theaterName}</p>
          <p>Screen: ${booking.screenName}</p>
          <p>Showtime: ${new Date(booking.showtime).toLocaleString()}</p>
          <p>Tickets:</p>
          <ul id="ticket-list">
            ${booking.bookedTickets
              .map(
                (ticket) => `
              <li>
                <div class="checkbox-wrapper-17">
                    <input type="checkbox" value=${ticket} id="${ticket}" />
                    <label for="${ticket}"></label>
                </div>
                <label id="ticketno" for="${ticket}">${ticket}</label> 
              </li>
            `
              )
              .join("")}
          </ul>
          <p>Total Price: ₹${booking.totalPrice}</p>
        `;
  };
  fetchBookingDetails(bookingId);

  document
    .getElementById("confirm-cancel")
    .addEventListener("click", async () => {
      const selectedTickets = Array.from(
        document.querySelectorAll("#ticket-list input:checked")
      ).map((checkbox) => checkbox.value);
      if (selectedTickets.length === 0) {
        alert("Please select at least one ticket to cancel.");
        return;
      }

      try {
        var bodydata = {
          bookingId,
          seatNumbers: selectedTickets,
          reason: "Nothing",
        };
        console.log(bodydata);
        const response = await fetch(
          `http://localhost:5091/api/Cancellation/ProcessCancellation`,
          {
            method: "POST",
            headers: {
              "Content-Type": "application/json",
              Authorization: `Bearer ${localStorage.getItem("token")}`,
            },
            body: JSON.stringify(bodydata),
          }
        );

        const result = await response.json();
        if (response.ok) {
          console.log(result);
          setTimeout(() => {
            Toastify({
              text: `Refund Amount : ${result.refundAmount} is been credited to your account in 2-3 business days`,
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

          setTimeout(() => {
            Toastify({
              text: `Cancellation Successfull !!`,
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
          }, 3000);

          setTimeout(() => {
            window.location.href = "booking_orders.html";
          }, 7000);
        } else {
          alert(result.message);
        }
      } catch (err) {
        console.error("Error cancelling booking:", err);
        alert("An error occurred while cancelling the booking.");
      }
    });
});
