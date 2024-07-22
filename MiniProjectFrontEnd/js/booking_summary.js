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
  const toggleUserMenu = () => {
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
  };

  document.querySelector(".user").addEventListener("click", toggleUserMenu);
  document
    .querySelector(".ad-nav-header-comp-2")
    .addEventListener("click", toggleUserMenu);

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
    const showtimeContainer = document.querySelector(".bscontainer");

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

  const urlParams = new URLSearchParams(window.location.search);
  const showtimeId = urlParams.get("showtimeid");
  const seats = urlParams.get("seats");
  const total = parseFloat(urlParams.get("total"));

  var baseamount = 130;
  var sgst = 23.4;
  var conveniencefees = baseamount + sgst;

  document.querySelectorAll(".grand-total").forEach((value) => {
    value.textContent = `₹ ${total + conveniencefees}`;
  });
  // Display seats and total price
  document.getElementById("summary-seats").textContent = seats;
  document.getElementById("summary-total").textContent = total;

  const snackItemsContainer = document.getElementById("snack-items");

  const fetchSnacks = () => {
    fetch("http://localhost:5091/api/Snack/GetAllSnacks", {
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
    })
      .then((res) => res.json())
      .then((data) => {
        console.log(data);
        snackItemsContainer.innerHTML = data
          .map((snack) => {
            return `
            <div class="snack-item">
              <div class="snack-image">
                <img src="${snack.snackImage}" alt="${snack.name}" />
              </div>
              <div class="snack-details">
                <div class="snack-detail-1">
                  <h4>${snack.name}</h4>
                  <p>${snack.description} (<span>${snack.calories} Kcal</span> | <span>Allergens: ${snack.allegers}</span>)</p>
                </div>
                <div class="snack-detail-2">
                  <p class="price">₹ ${snack.price}</p>
                  <div class="addorremovediv">
                    <button class="snack-add-btn" data-id="${snack.snackId}" data-name="${snack.name}" data-price="${snack.price}">Add</button>
                    <div class="quantity-controller hidden" data-id="${snack.snackId}">
                      <button class="snack-decrease-btn" data-id="${snack.snackId}" data-name="${snack.name}" data-price="${snack.price}">-</button>
                      <span class="quantity">0</span>
                      <button class="snack-increase-btn" data-id="${snack.snackId}" data-name="${snack.name}" data-price="${snack.price}">+</button>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          `;
          })
          .join("");
        addSnackButtonListeners();
        addSnackIncreaseButtonListeners();
        addSnackDecreaseButtonListeners();
      });
  };
  fetchSnacks();

  const selectedSnacks = [];
  let snacksTotal = 0;

  const addSnackButtonListeners = () => {
    document.querySelectorAll(".snack-add-btn").forEach((button) => {
      button.addEventListener("click", (event) => {
        const snack = {
          id: event.target.dataset.id,
          name: event.target.dataset.name,
          price: parseFloat(event.target.dataset.price),
        };
        addSnack(snack);
      });
    });
  };

  const addSnackIncreaseButtonListeners = () => {
    document.querySelectorAll(".snack-increase-btn").forEach((button) => {
      button.addEventListener("click", (event) => {
        const snack = {
          id: event.target.dataset.id,
          name: event.target.dataset.name,
          price: parseFloat(event.target.dataset.price),
        };
        increaseSnack(snack);
      });
    });
  };

  const addSnackDecreaseButtonListeners = () => {
    document.querySelectorAll(".snack-decrease-btn").forEach((button) => {
      button.addEventListener("click", (event) => {
        const snack = {
          id: event.target.dataset.id,
          name: event.target.dataset.name,
          price: parseFloat(event.target.dataset.price),
        };
        decreaseSnack(snack);
      });
    });
  };

  const conveniencefeesdiv = document.querySelector(".convinence-fees");
  conveniencefeesdiv.addEventListener("click", () => {
    const insidecollapsed = document.querySelector(".inside-collapsed");
    if (!insidecollapsed.classList.contains("actived")) {
      insidecollapsed.classList.add("actived");
    } else {
      insidecollapsed.classList.remove("actived");
    }
  });

  const foodbeveragediv = document.querySelector(".foodbeverage");
  foodbeveragediv.addEventListener("click", () => {
    const seletedsnacksdiv = document.getElementById("selected-snacks");
    if (!seletedsnacksdiv.classList.contains("actived")) {
      seletedsnacksdiv.classList.add("actived");
    } else {
      seletedsnacksdiv.classList.remove("actived");
    }
  });

  function addSnack(snack) {
    console.log(selectedSnacks);
    selectedSnacks.push(snack);
    snacksTotal += snack.price;

    document.getElementById("snacks-total").textContent = `₹ ${snacksTotal}`;
    updateGrandTotal();

    // Show the quantity controller and update quantity
    const quantityController = document.querySelector(
      `.quantity-controller[data-id="${snack.id}"]`
    );
    const addButton = document.querySelector(
      `.snack-add-btn[data-id="${snack.id}"]`
    );

    if (quantityController) {
      quantityController.classList.remove("hidden");
      const quantitySpan = quantityController.querySelector(".quantity");
      quantitySpan.textContent = parseInt(quantitySpan.textContent) + 1;
    }

    addButton.classList.add("hidden");

    // Check if snack is already in the list
    let existingSnack = document.querySelector(
      `#selected-snacks li[data-id="${snack.id}"]`
    );

    if (existingSnack) {
      let existingQuantity = existingSnack.querySelector(".quantity");
      let newQuantity = parseInt(existingQuantity.textContent) + 1;
      existingQuantity.textContent = newQuantity;

      let existingTotalPrice = existingSnack.querySelector(".total-price");
      existingTotalPrice.textContent = `₹ ${newQuantity * snack.price}`;
    } else {
      const selectedSnacksContainer =
        document.getElementById("selected-snacks");
      const snackItem = document.createElement("li");
      snackItem.dataset.id = snack.id;
      snackItem.innerHTML = `${snack.name} (<span class="quantity">1</span>) - <span class="total-price">₹ ${snack.price}</span>`;
      selectedSnacksContainer.appendChild(snackItem);
    }
  }

  function increaseSnack(snack) {
    console.log(selectedSnacks);
    selectedSnacks.push(snack);
    snacksTotal += snack.price;

    document.getElementById("snacks-total").textContent = `₹ ${snacksTotal}`;
    updateGrandTotal();

    const quantityController = document.querySelector(
      `.quantity-controller[data-id="${snack.id}"]`
    );
    const quantitySpan = quantityController.querySelector(".quantity");

    quantitySpan.textContent = parseInt(quantitySpan.textContent) + 1;

    let existingSnack = document.querySelector(
      `#selected-snacks li[data-id="${snack.id}"]`
    );

    if (existingSnack) {
      let existingQuantity = existingSnack.querySelector(".quantity");
      let newQuantity = parseInt(existingQuantity.textContent) + 1;
      existingQuantity.textContent = newQuantity;

      let existingTotalPrice = existingSnack.querySelector(".total-price");
      existingTotalPrice.textContent = `₹ ${newQuantity * snack.price}`;
    }
  }

  function decreaseSnack(snack) {
    const index = selectedSnacks.findIndex((s) => s.id === snack.id);
    if (index !== -1) {
      selectedSnacks.splice(index, 1);
      snacksTotal -= snack.price;

      document.getElementById("snacks-total").textContent = `₹ ${snacksTotal}`;
      updateGrandTotal();

      const quantityController = document.querySelector(
        `.quantity-controller[data-id="${snack.id}"]`
      );
      const quantitySpan = quantityController.querySelector(".quantity");

      quantitySpan.textContent = parseInt(quantitySpan.textContent) - 1;

      if (parseInt(quantitySpan.textContent) === 0) {
        quantityController.classList.add("hidden");
        document
          .querySelector(`.snack-add-btn[data-id="${snack.id}"]`)
          .classList.remove("hidden");
      }

      let existingSnack = document.querySelector(
        `#selected-snacks li[data-id="${snack.id}"]`
      );

      if (existingSnack) {
        let existingQuantity = existingSnack.querySelector(".quantity");
        let newQuantity = parseInt(existingQuantity.textContent) - 1;
        existingQuantity.textContent = newQuantity;

        let existingTotalPrice = existingSnack.querySelector(".total-price");
        existingTotalPrice.textContent = `₹ ${newQuantity * snack.price}`;

        if (newQuantity === 0) {
          existingSnack.remove();
        }
      }
    }
  }

  function updateGrandTotal() {
    document.querySelectorAll(".grand-total").forEach((value) => {
      value.textContent = `₹ ${total + snacksTotal + conveniencefees}`;
    });
  }

  const fetchScreenandTheater = () => {
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
        console.log("It came in");
        const screenname = document.querySelector(".screen-name");
        screenname.innerHTML = data.screenName;
      });
  };
  fetchScreenandTheater();

  const fetchMovie = async () => {
    try {
      const response = await fetch(
        `http://localhost:5091/api/Showtime/GetMovieByShowtimeId?showtimeid=${parseInt(
          showtimeId
        )}`,
        {
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
        }
      );
      const data = await response.json();
      console.log(data.movieId);
      return data.movieId;
    } catch (error) {
      console.error("Error fetching movie:", error);
      return null;
    }
  };

  document
    .getElementById("confirm-booking")
    .addEventListener("click", async () => {
      const userId = parseInt(localStorage.getItem("uid"));
      const pointsToRedeem = 0;
      const movieId = await fetchMovie();
      console.log(movieId);

      let bookingSnacks = {};
      selectedSnacks.forEach((snack) => {
        if (bookingSnacks[snack.name]) {
          bookingSnacks[snack.name]++;
        } else {
          bookingSnacks[snack.name] = 1;
        }
      });

      console.log(bookingSnacks);
      console.log(movieId);
      console.log(seats);

      var seatsboo = seats.split(",").map((seat) => seat.trim());
      console.log(seatsboo);

      const bookingData = {
        userId,
        showtimeId: parseInt(showtimeId),
        movieId: 1,
        bookingSeats: seatsboo,
        bookingSnacks,
      };
      console.log(bookingData);

      fetch("http://localhost:5091/api/Booking/AddBooking", {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${localStorage.getItem("token")}`,
        },
        body: JSON.stringify(bookingData),
      })
        .then((response) => response.json())
        .then((data) => {
          console.log("Booking successful:", data);
          if (data.bookingId !== undefined) {
            sessionStorage.setItem("seats", seatsboo.join(", "));
            sessionStorage.setItem("snacks", JSON.stringify(selectedSnacks));
            window.location.href = `payment.html?showtimeId=${showtimeId}&bookingId=${encodeURIComponent(
              data.bookingId
            )}`;
          } else {
            Toastify({
              text: "Something went wrong!!",
              duration: 3000,
              close: true,
              gravity: "top",
              position: "right",
              backgroundColor: "linear-gradient(to right, #FF7F7F, #E4003A)",
            }).showToast();
          }
        })
        .catch((error) => {
          console.error("Error booking tickets:", error);
          // Display an error message to the user
        });
    });
});
