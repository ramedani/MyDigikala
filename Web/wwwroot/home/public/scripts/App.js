// Select DOM elements
const nav = document.querySelector('.mobile-menu');
const openNavBtn = document.querySelector('.mobile-menu__open-icon');
const closeNavBtn = document.querySelector('.mobile-menu__close-icon');
const openSubmenuBtn = document.querySelector('.open-submenu');
const submenu = document.querySelector('.submenu');
const arrowsubmenu = document.querySelector('.arrow-submenu');
const overlay = document.querySelector('.overlay');
const openShoppingCartButtons = document.querySelectorAll('.open-shopping-cart__btn');
const shoppingCart = document.querySelector('.shopping-cart');
const closeShoppingCartButtons = document.querySelectorAll('.close-shopping-cart__btn');
const alertElem = document.querySelector('.top-alert');
const alertBtnElem = document.querySelector('.close-alert-btn');
const toggleThemeBtns = document.querySelectorAll('.toggle-theme');


// Function to open mobile menu
const openNav = () => {
  overlay.classList.remove('hidden');
  overlay.classList.add('flex');
  nav.classList.remove('-right-64');
  nav.classList.add('right-0');
};

// Function to open shopping cart
const openShoppingCart = () => {
  overlay.classList.remove('hidden');
  overlay.classList.add('flex');
  shoppingCart.classList.remove('-left-72');
  shoppingCart.classList.add('left-0');
};

// Function to close mobile menu
const closeNav = () => {
  overlay.classList.add('hidden');
  overlay.classList.remove('flex');
  nav.classList.remove('right-0');
  nav.classList.add('-right-64');
};

// Function to close shopping cart
const closeShoppingCart = () => {
  overlay.classList.add('hidden');
  overlay.classList.remove('flex');
  shoppingCart.classList.add('-left-72');
  shoppingCart.classList.remove('left-0');
};

// Function to toggle submenu
const toggleSubmenu = () => {
  openSubmenuBtn.classList.toggle('text-green-500');
  submenu.classList.toggle('hidden');
  submenu.classList.toggle('flex');
  arrowsubmenu.classList.toggle('-rotate-90');
};


// Function to close alert
const closeAlert = () => {
  alertElem.classList.add('hidden');
};

// Function to toggle theme
const toggleTheme = () => {
  if (localStorage.theme === 'dark') {
    document.documentElement.classList.remove('dark');
    localStorage.theme = 'light';
  } else {
    document.documentElement.classList.add('dark');
    localStorage.setItem('theme', 'dark');
  }
};

// Event Listeners

// Event listener for opening mobile menu
openNavBtn.addEventListener('click', openNav);

// Event listener for opening shopping cart
openShoppingCartButtons.forEach(button => {
  button.addEventListener('click', openShoppingCart);
});

// Event listener for closing mobile menu
closeNavBtn.addEventListener('click', closeNav);

// Event listener for closing shopping cart
closeShoppingCartButtons.forEach(button => {
  button.addEventListener('click', closeShoppingCart);
});

// Event listener for overlay click to close mobile menu and shopping cart
overlay.addEventListener('click', () => {
  closeNav();
  closeShoppingCart();
});

// Event listener for submenu toggle
openSubmenuBtn.addEventListener('click', toggleSubmenu);

// Event listener for closing alert
alertBtnElem.addEventListener('click', closeAlert);

// Event listeners for theme toggle
toggleThemeBtns.forEach(btn => {
  btn.addEventListener('click', toggleTheme);
});


const accordionHeaders = document.querySelectorAll('.accordion-header');

accordionHeaders.forEach(header => {
  header.addEventListener('click', function () {
    const content = this.nextElementSibling;
    const icon = this.querySelector('svg');

    if (content.classList.contains('hidden')) {
      content.classList.remove('hidden');
      content.classList.add('block');
      icon.classList.add('rotate-90');
    } else {
      content.classList.remove('block');
      content.classList.add('hidden');
      icon.classList.remove('rotate-90');
    }
  });
});



// Customizes an input field to use SVG icons for incrementing/decrementing its value between 1 and 20.
document.addEventListener('DOMContentLoaded', () => {
  const input = document.getElementById('customInput');
  const incrementButton = document.querySelector('.increment');
  const decrementButton = document.querySelector('.decrement');

  incrementButton?.addEventListener('click', () => {
    if (input.value < 20) {
      input.value = parseInt(input.value) + 1;
    }
  });

  decrementButton?.addEventListener('click', () => {
    if (input.value > 1) {
      input.value = parseInt(input.value) - 1;
    }
  });
});


// SHOW MORE COMMENTS
const moreCommentBtn = document.querySelector('.more-comment-btn');
const moreCommentText = document.querySelector('.more-comment-text');
const moreCommentIcon = document.querySelector('.more-comment-icon');
const hiddenCommentItems = document.querySelectorAll('.hidden-comment-item');

if (moreCommentBtn) {
  moreCommentBtn.addEventListener('click', () => {
    hiddenCommentItems.forEach(item => {
      item.classList.toggle('hidden');
      item.classList.toggle('block');
    });

    if (moreCommentText.innerHTML === 'مشاهده بیشتر') {
      moreCommentText.innerHTML = 'مشاهده کمتر';
    } else {
      moreCommentText.innerHTML = 'مشاهده بیشتر';
    }

    moreCommentIcon.classList.toggle('rotate-180');
  });
}


// PRICE RANGE
const priceElements = document.querySelectorAll(".price-input p");
const rangeInputs = document.querySelectorAll(".range-input input");
const range = document.querySelector(".slider-bar .progress");

let priceGap = 1000;

// تابع برای فرمت کردن اعداد به صورت سه‌رقمی
function formatNumber(num) {
  return num.toString().replace(/\B(?=(\d{3})+(?!\d))/g, ",");
}

rangeInputs.forEach((input) => {
  input.addEventListener("input", (e) => {
    let minVal = parseInt(rangeInputs[0].value) * 10; // ضرب در 10 برای تبدیل به ده‌ها هزار
    let maxVal = parseInt(rangeInputs[1].value) * 10;

    if (maxVal - minVal < priceGap) {
      if (e.target.className === "min-range") {
        rangeInputs[0].value = (maxVal - priceGap) / 10; // تبدیل به مقدار اولیه
      } else {
        rangeInputs[1].value = (minVal + priceGap) / 10; // تبدیل به مقدار اولیه
      }
    } else {
      priceElements[0].textContent = formatNumber(minVal);
      priceElements[1].textContent = formatNumber(maxVal);
      range.style.left = (rangeInputs[0].value / rangeInputs[0].max) * 100 + "%";
      range.style.right = 100 - (rangeInputs[1].value / rangeInputs[1].max) * 100 + "%";
    }
  });
});


function showAlert(type) {
  const isDarkMode = document.documentElement.classList.contains('dark');

  // تنظیمات پیش‌فرض برای حالت دارک و لایت
  const alertOptions = {
    background: isDarkMode ? '#333' : '#fff',
    color: isDarkMode ? '#fff' : '#000',
    confirmButtonColor: isDarkMode ? '#4caf50' : '#3085d6',
    cancelButtonColor: isDarkMode ? '#f44336' : '#d33',
  };

  switch (type) {
    case 'success-register':
      Swal.fire({
        icon: 'success',
        title: 'ثبت‌نام موفق!',
        text: 'حساب کاربری شما با موفقیت ایجاد شد.',
        confirmButtonText: 'باشه',
        ...alertOptions,

      });
      break;
    case 'error-login':
      Swal.fire({
        icon: 'error',
        title: 'خطا!',
        text: 'نام کاربری یا رمز عبور اشتباه است.',
        confirmButtonText: 'تلاش مجدد',
        ...alertOptions,

      });
      break;
    case 'add-product':
      Swal.fire({
        icon: 'success',
        title: 'محصول اضافه شد!',
        text: 'محصول به سبد خرید شما اضافه شد.',
        confirmButtonText: 'ادامه خرید',
        ...alertOptions,

      });
      break;
    case 'delete-product':
      Swal.fire({
        title: 'حذف محصول',
        text: 'آیا از حذف این محصول از سبد خرید مطمئن هستید؟',
        icon: 'warning',
        showCancelButton: true,
        confirmButtonText: 'بله، حذف کن',
        ...alertOptions,

        cancelButtonText: 'لغو'
      });
      break;
  }
}