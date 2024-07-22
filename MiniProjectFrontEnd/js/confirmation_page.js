document.addEventListener("DOMContentLoaded", () => {
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
        document.querySelector(".bc-container").style.opacity = "1.0";
      }
    }
  });

  var locationdown = document.querySelector(".down-button");
  locationdown.addEventListener("click", () => {
    var popuplocation = document.querySelector(".popup-location");
    if (popuplocation.classList.contains("active")) {
      document.querySelector(".bc-container").style.opacity = "1.0";
      popuplocation.classList.remove("active");
    } else {
      document.querySelector(".bc-container").style.opacity = "0.3";
      popuplocation.classList.add("active");
    }
  });

  const cityButtons = document.querySelectorAll(".pcities button");
  const locationSpan = document.querySelector(".location span");

  cityButtons.forEach((button) => {
    button.addEventListener("click", () => {
      var popuplocation = document.querySelector(".popup-location");
      document.querySelector(".bc-container").style.opacity = "1.0";
      popuplocation.classList.remove("active");
      locationSpan.innerHTML = button.innerHTML;
    });
  });
  const urlParams = new URLSearchParams(window.location.search);

  const fetchpayment = () => {
    fetch(
      `http://localhost:5091/api/Payment/GetPaymentById?paymentId=${urlParams.get(
        "paymentId"
      )}`,
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
        const bookingDetails = JSON.parse(
          localStorage.getItem("bookingDetails")
        );

        if (bookingDetails) {
          document.getElementById("movie-name").innerText =
            bookingDetails.movieName;
          document.getElementById("noofticket").innerText =
            bookingDetails.tickets.split(",").length;
          document.getElementById("theater-name").innerText =
            bookingDetails.theaterName;
          document.getElementById("screen-name").innerText =
            bookingDetails.screenName;
          document.getElementById("showtime").innerText =
            bookingDetails.showtime;
          document.getElementById("booked-tickets").innerText =
            bookingDetails.tickets;
          console.log(bookingDetails);
          document.getElementById("booked-snacks").innerHTML = JSON.parse(
            bookingDetails.snacks
          )
            .map((snack, index) => {
              return `<span>${snack.name}</span>`;
            })
            .join("");
          document.getElementById(
            "amount"
          ).innerText = `Rs. ${data.orderTotal}`;
        } else {
          alert("No booking details found!");
        }
      });
  };
  fetchpayment();
});
