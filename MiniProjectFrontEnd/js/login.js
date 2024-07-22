document
  .getElementById("forgotPasswordLink")
  .addEventListener("click", function (event) {
    event.preventDefault();
    document.getElementById("loginWrapper").style.display = "none";
    document.getElementById("forgotPasswordWrapper").style.display = "block";
  });

document.addEventListener("DOMContentLoaded", () => {
  if (localStorage.getItem("token")) {
    window.location.href = "index.html";
  }
});

document
  .getElementById("forgotPasswordForm")
  .addEventListener("submit", function (event) {
    event.preventDefault();

    const email = document.getElementById("forgotEmail").value;

    if (!email) {
      showToast("Email is required.");
      return;
    } else if (!validateEmail(email)) {
      showToast("Invalid email format.");
      return;
    }
  });

document
  .getElementById("loginForm")
  .addEventListener("submit", function (event) {
    event.preventDefault();

    // Get form values
    const email = document.getElementById("loginEmail").value;
    const password = document.getElementById("loginPassword").value;

    // Validate inputs
    const errors = [];

    if (!email) {
      errors.push("Email is required.");
    } else if (!validateEmail(email)) {
      errors.push("Invalid email format.");
    }

    if (!password) {
      errors.push("Password is required.");
    }

    if (errors.length > 0) {
      showToast(errors.join("\n"));
      return;
    }

    // Send data to the endpoint
    const data = {
      UserEmail: email,
      password: password,
    };

    fetch("http://localhost:5091/api/User/Login", {
      method: "POST",
      headers: {
        "Content-Type": "application/json",
        Authorization: `Bearer ${localStorage.getItem("token")}`,
      },
      body: JSON.stringify(data),
    })
      .then((response) => {
        if (response.ok) {
          return response.json();
        } else if (response.status === 401) {
          throw new Error("Unauthorized: Incorrect email or password.");
        } else if (response.status === 409) {
          return response.json().then((errorData) => {
            throw new Error(errorData.message || "Conflict occurred.");
          });
        } else {
          return response.json().then((errorData) => {
            throw new Error(errorData.message || "Login failed.");
          });
        }
      })
      .then((data) => {
        Toastify({
          text: "Login Success !! Your Logged In",
          duration: 3000,
          newWindow: true,
          close: true,
          gravity: "top", // `top` or `bottom`
          position: "right", // `left`, `center` or `right`
          stopOnFocus: true, // Prevents dismissing of toast on hover
          style: {
            background: "linear-gradient(to right, #BDFF6C, #A7FF3B,#8FFE09)",
            color: "black",
          },
          onClick: function () {}, // Callback after click
        }).showToast();

        localStorage.setItem("token", data.token);
        localStorage.setItem("uid", data.userId);

        setTimeout(() => {
          window.location.href = "index.html";
        }, 1500);
      })
      .catch((error) => {
        console.error("Error:", error);
        Toastify({
          text: "Login Failed !! Check your password",
          duration: 3000,
          newWindow: true,
          close: true,
          gravity: "top", // `top` or `bottom`
          position: "right", // `left`, `center` or `right`
          stopOnFocus: true, // Prevents dismissing of toast on hover
          style: {
            background: "linear-gradient(to right, #FF7074, #FF474D,#C6373C)",
          },
          onClick: function () {}, // Callback after click
        }).showToast();
      });
  });
function validateEmail(email) {
  const re = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
  return re.test(String(email).toLowerCase());
}
