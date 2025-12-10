// Elements Selection
const toggleThemeBtns = document.querySelectorAll('.toggle-theme');
const openSidebarBtn = document.querySelector('.open-sidebar-btn');
const closeSidebarBtn = document.querySelector('.close-sidebar-btn');
const sidebarMobile = document.querySelector('.sidebar-mobile');
const overlay = document.querySelector('.overlay');
const notifBox = document.querySelector('.notif-box');
const notifToggleBtn = document.querySelector('.notif-toggle-btn');

// Theme Toggling Function
const toggleTheme = () => {
    const isDark = localStorage.theme === 'dark';
    document.documentElement.classList.toggle('dark', !isDark);
    localStorage.theme = isDark ? 'light' : 'dark';
};

// Sidebar Toggling Function
const toggleSidebar = (isOpen) => {
    sidebarMobile.classList.toggle('-right-64', !isOpen);
    sidebarMobile.classList.toggle('right-0', isOpen);
    overlay.classList.toggle('hidden', !isOpen);
    overlay.classList.toggle('flex', isOpen);
};

// Notification Toggling Function
const toggleNotification = (event) => {
    const isNotifBoxClicked = notifBox.contains(event.target);
    const isNotifToggleBtnClicked = notifToggleBtn.contains(event.target);
    
    // If notification box is open and clicked outside, close it
    if (!isNotifToggleBtnClicked && !isNotifBoxClicked) {
        notifBox.classList.add('hidden');
    } 
};

// Toggle notification box when the bell icon is clicked
notifToggleBtn.addEventListener('click', (event) => {
    event.stopPropagation();
    notifBox.classList.toggle('hidden');
});

// Close the notification box when clicking outside of it
document.addEventListener('click', (event) => {
    toggleNotification(event);
});

// Additional Event Listeners for Sidebar and Theme Toggling (if needed)
toggleThemeBtns.forEach(btn => btn.addEventListener('click', toggleTheme));

openSidebarBtn.addEventListener('click', () => toggleSidebar(true));
closeSidebarBtn.addEventListener('click', () => toggleSidebar(false));
overlay.addEventListener('click', () => toggleSidebar(false));
