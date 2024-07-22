document.addEventListener("DOMContentLoaded", () => {
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
    const showtimeContainer = document.querySelector(".scontainer");

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
    const showtimeContainer = document.querySelector(".scontainer");

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
      const showtimeContainer = document.querySelector(".scontainer");

      showtimeContainer.style.opacity = "1.0";
      popupLocation.classList.remove("active");
      locationSpan.innerHTML = button.innerHTML;
    });
  });

  const urlParams = new URLSearchParams(window.location.search);
  const showtimeId = urlParams.get("showtimeid");

  if (showtimeId) {
    fetchSeats(showtimeId);
  } else {
    console.error("Showtime ID not found in the URL");
  }
});
var seatselected = document.getElementById("count");
var observer = new MutationObserver(function (mutations) {
  mutations.forEach(function (mutation) {
    if (mutation.type === "childList") {
      if (seatselected.innerHTML != "0") {
        document.querySelector(".paynow").style.display = "flex";
      } else {
        document.querySelector(".paynow").style.display = "none";
      }
    }
  });
});

// Configure the observer to listen for changes in the child list of the seatselected element
observer.observe(seatselected, { childList: true });

function fetchSeats(showtimeId) {
  fetch(
    `http://localhost:5091/api/ShowtimeSeat/GetShowtimeSeats?showtimeid=${showtimeId}`,
    {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    }
  )
    .then((response) => response.json())
    .then((data) => {
      console.log(data);
      generateSeats(data);
    })
    .catch((error) => console.error("Error fetching seats:", error));
}

let selectedSeats = [];
let seatPrices = {};
let totalPrice = 0;

function generateSeats(seats) {
  const containerseat = document.querySelector(".container-seat");
  containerseat.innerHTML = "";

  seats.forEach((seat) => {
    const row = seat.seatNumber[0];
    const seatNum = seat.seatNumber.slice(1);
    const seatPrice = seat.price;

    // Store seat prices
    seatPrices[seat.seatNumber] = seatPrice;

    let rowDiv = containerseat.querySelector(`.row[data-row="${row}"]`);

    if (!rowDiv) {
      rowDiv = document.createElement("div");
      rowDiv.classList.add("row");
      rowDiv.dataset.row = row;

      const rowNum = document.createElement("p");
      rowNum.classList.add("rownum");
      rowNum.textContent = row;

      rowDiv.appendChild(rowNum);
      containerseat.appendChild(rowDiv);
    }

    const seatDiv = document.createElement("div");
    seatDiv.classList.add("seat");
    seatDiv.textContent = seatNum;
    seatDiv.setAttribute("data-price", `Price: ${seatPrice} Rs`);

    if (seat.status === "Booked") {
      seatDiv.classList.add("sold");
    } else if (seat.status === "Selected") {
      seatDiv.classList.add("selected");
      selectedSeats.push(seat.seatNumber);
    }

    seatDiv.addEventListener("click", () =>
      selectSeat(seatDiv, seat.seatNumber)
    );
    seatDiv.addEventListener("mouseover", () => showSeatPrice(seatPrice));
    seatDiv.addEventListener("mouseout", hideSeatPrice);

    rowDiv.appendChild(seatDiv);
  });

  updateSummary();
}

const paynowbtn = document.querySelector(".paynow");

paynowbtn.addEventListener("click", () => {
  const showtimeContainer = document.querySelector(".scontainer");
  var popupModal = document.getElementById("popup-modal");
  if (!popupModal.classList.contains("active")) {
    popupModal.classList.add("active");
    showtimeContainer.style.opacity = "0.2";
  } else {
    popupModal.classList.remove("active");
    showtimeContainer.style.opacity = "1";
  }

  var acceptbtn = document.getElementById("accept-btn");
  acceptbtn.addEventListener("click", () => {
    popupModal.classList.remove("active");
    showtimeContainer.style.opacity = "1";

    // Store selected tickets and total amount in localStorage
    localStorage.setItem(
      "selectedticketcount",
      document.getElementById("count").textContent
    );
    localStorage.setItem(
      "seatnumbers",
      document.getElementById("tickets").textContent
    );
    localStorage.setItem(
      "totalamount",
      document.getElementById("total").textContent
    );

    const urlParams = new URLSearchParams(window.location.search);
    // Redirect to booking summary page with URL parameters
    const showtimeId = urlParams.get("showtimeid");
    const seatNumbers = document.getElementById("tickets").textContent;
    const totalAmount = document.getElementById("total").textContent;

    window.location.href = `bookingsummary.html?showtimeid=${showtimeId}&seats=${encodeURIComponent(
      seatNumbers
    )}&total=${totalAmount}`;
  });
  var cancelbtn = document.getElementById("cancel-btn");
  cancelbtn.addEventListener("click", () => {
    popupModal.classList.remove("active");
    showtimeContainer.style.opacity = "1";
  });
});

function selectSeat(seatDiv, seatNumber) {
  if (seatDiv.classList.contains("sold")) {
    return;
  }

  seatDiv.classList.toggle("selected");

  if (seatDiv.classList.contains("selected")) {
    selectedSeats.push(seatNumber);
  } else {
    const index = selectedSeats.indexOf(seatNumber);
    if (index > -1) {
      selectedSeats.splice(index, 1);
    }
  }

  updateSummary();
}

function updateSummary() {
  const count = selectedSeats.length;
  const tickets = selectedSeats.join(", ");
  totalPrice = selectedSeats.reduce(
    (total, seat) => total + seatPrices[seat],
    0
  );

  document.getElementById("count").textContent = count;
  document.getElementById("tickets").textContent = tickets;
  document.getElementById("total").textContent = totalPrice;
}

function showSeatPrice(price) {
  // Implement code to show seat price (e.g., in a tooltip or a dedicated element)
  console.log(`Seat price: ${price}`);
}

function hideSeatPrice() {
  // Implement code to hide seat price
  console.log("Hide seat price");
}
