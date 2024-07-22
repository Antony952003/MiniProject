document.addEventListener("DOMContentLoaded", () => {
  document.querySelector(".ad-nav-content").addEventListener("click", () => {
    window.location.href = "booking_orders.html";
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
    }
  });

  document.querySelectorAll(".tohome").forEach((e) => {
    e.addEventListener("click", () => {
      window.location.href = "index.html";
    });
  });

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

  //   Fetching user details from backedn using the uid in the localstorage

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
        populateForm(data);
      });
  };
  fetchuserdetails();

  const form = document.getElementById("user-form");
  const editButton = document.getElementById("edit-btn");
  const saveButton = document.getElementById("save-btn");
  const clearButton = document.getElementById("clear-btn");
  const inputs = form.querySelectorAll("input");

  // Populate the form with initial data
  function populateForm(data) {
    form.name.value = data.name;
    form.email.value = data.email;
    form.phone.value = data.phone;
    form["imageurl"].value = data.image;
  }

  editButton.addEventListener("click", () => {
    if (inputs[0].hasAttribute("readonly"))
      inputs.forEach((input) => input.removeAttribute("readonly"));
    else inputs.forEach((input) => input.toggleAttribute("readonly"));
  });

  saveButton.addEventListener("click", async () => {
    const formData = new FormData(form);
    let imageUrl = formData.get("imageurl");
    const imageFile = formData.get("image");

    if (imageFile && imageFile.size > 0) {
      const base64Image = await convertToBase64(imageFile);
      imageUrl = base64Image;
    }

    const data = {
      id: localStorage.getItem("uid"),
      name: formData.get("name"),
      email: formData.get("email"),
      phone: formData.get("phone"),
      image: imageUrl,
    };

    try {
      const response = await fetch(
        "http://localhost:5091/api/User/UpdateUserDetails",
        {
          method: "PUT",
          headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${localStorage.getItem("token")}`,
          },
          body: JSON.stringify(data),
        }
      );

      if (response.ok) {
        console.log("Data saved successfully");
        Toastify({
          text: "UserDetails Updated Successfully",
          duration: 3000,
          newWindow: true,
          close: true,
          gravity: "top",
          position: "right",
          stopOnFocus: true,
          style: {
            background: "linear-gradient(to right, #BDFF6C, #A7FF3B,#8FFE09)",
            color: "black",
          },
          onClick: function () {},
        }).showToast();
        inputs.forEach((input) => input.setAttribute("readonly", true));
        fetchuserdetails();
      } else {
        console.error("Error saving data");
      }
    } catch (error) {
      console.error("Error:", error);
    }
  });

  clearButton.addEventListener("click", () => {
    inputs.forEach((input) => (input.value = ""));
  });

  function convertToBase64(file) {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();
      reader.readAsDataURL(file);
      reader.onload = () => resolve(reader.result);
      reader.onerror = (error) => reject(error);
    });
  }
});
