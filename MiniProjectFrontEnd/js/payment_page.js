document.addEventListener("DOMContentLoaded", () => {
  const loader = document.getElementById("loader");

  // Function to show loader
  const showLoader = () => {
    loader.classList.add("active");
  };

  // Function to hide loader
  const hideLoader = () => {
    loader.classList.remove("active");
  };
  var selectedMethod = "Credit Card";

  document.querySelectorAll('input[name="payment-method"]').forEach((input) => {
    input.addEventListener("change", (event) => {
      selectedMethod = event.target.value;
      document.querySelectorAll(".payment-details").forEach((div) => {
        // Remove 'required' attribute from hidden fields
        div.querySelectorAll("input").forEach((input) => {
          input.removeAttribute("required");
        });
        div.classList.add("hidden");
      });
      const activeDiv = document.getElementById(`details-${selectedMethod}`);
      activeDiv.classList.remove("hidden");
      // Add 'required' attribute to visible fields
      activeDiv.querySelectorAll("input").forEach((input) => {
        input.setAttribute("required", true);
      });
    });
  });

  const urlParams = new URLSearchParams(window.location.search);
  const bookingId = urlParams.get("bookingId");
  const showtimeId = urlParams.get("showtimeId");

  const fetchBooking = () => {
    showLoader();
    fetch(
      `http://localhost:5091/api/Booking/GetBookingById?bookingid=${parseInt(
        bookingId
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
        hideLoader();
        const theatername = document.getElementById("theater-name");
        theatername.innerHTML = data.theaterName;
        const screenname = document.getElementById("screen-name");
        screenname.innerHTML = data.screenName;
        const moviename = document.getElementById("movie-name");
        moviename.innerHTML = data.movieName;
        const tickets = document.getElementById("tickets");
        tickets.innerHTML = data.bookedTickets.join(", ");
      });
  };

  function formatDateTime(dateTimeStr) {
    const date = new Date(dateTimeStr);

    const day = date.getDate();
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
    const month = monthNames[date.getMonth()];
    const year = date.getFullYear();

    let hours = date.getHours();
    const minutes = date.getMinutes();
    const ampm = hours >= 12 ? "PM" : "AM";
    hours = hours % 12;
    hours = hours ? hours : 12; // the hour '0' should be '12'

    // Add ordinal suffix to the day
    const daySuffix = (day) => {
      if (day > 3 && day < 21) return "th"; // covers 11th to 20th
      switch (day % 10) {
        case 1:
          return "st";
        case 2:
          return "nd";
        case 3:
          return "rd";
        default:
          return "th";
      }
    };

    return `${day}${daySuffix(day)} ${month} ${hours}:${
      minutes < 10 ? "0" + minutes : minutes
    } ${ampm}`;
  }

  fetchBooking();

  const fetchShowtime = () => {
    fetch(
      `http://localhost:5091/api/Showtime/GetShowtimeById?showtimeid=${parseInt(
        showtimeId
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
        const showtimename = document.getElementById("showtime");
        showtimename.innerHTML = formatDateTime(data.startTime);
      });
  };
  fetchShowtime();

  const processPayment = () => {
    showLoader();
    fetch(`http://localhost:5091/api/Payment/ProcessPayment`, {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify({
        bookingId: bookingId,
        paymentMethod: selectedMethod,
      }),
    })
      .then((res) => res.json())
      .then((data) => {
        hideLoader();
        console.log(data);
        if (data.hasOwnProperty("paymentId")) {
          Toastify({
            text: "Payment Successful",
            duration: 3000,
            close: true,
            gravity: "top",
            position: "right",
            backgroundColor: "linear-gradient(to right, #00b09b, #96c93d)",
          }).showToast();
          // Store booking details in localStorage
          localStorage.setItem(
            "bookingDetails",
            JSON.stringify({
              theaterName: document.getElementById("theater-name").innerText,
              movieName: document.getElementById("movie-name").innerText,
              screenName: document.getElementById("screen-name").innerText,
              tickets: document.getElementById("tickets").innerText,
              showtime: document.getElementById("showtime").innerText,
              snacks: sessionStorage.getItem("snacks"),
            })
          );

          window.location.href = `booking_confirmation.html?paymentId=${data.paymentId}`;
        } else {
          Toastify({
            text: "Already Paid!!",
            duration: 3000,
            close: true,
            gravity: "top",
            position: "right",
            backgroundColor: "linear-gradient(to right, #FF7F7F, #E4003A)",
          }).showToast();
        }
      });
  };
  // Handle form submission
  document
    .getElementById("payment-form")
    .addEventListener("submit", (event) => {
      event.preventDefault();
      processPayment();
    });
});
