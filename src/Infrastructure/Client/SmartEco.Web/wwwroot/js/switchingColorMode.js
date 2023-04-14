//identify the toggle switch HTML element
const toggleSwitch = document.querySelector('#switcherDarkMode');

//listener for changing themes
toggleSwitch.addEventListener('change', switchTheme, false);

//function that changes the theme, and sets a localStorage variable to track the theme between page loads
function switchTheme(e) {
    if (e.target.checked) {
        localStorage.setItem('theme', 'dark');
        document.documentElement.setAttribute('data-bs-theme', 'dark');
        toggleSwitch.checked = true;
    } else {
        localStorage.setItem('theme', 'light');
        document.documentElement.setAttribute('data-bs-theme', 'light');
        toggleSwitch.checked = false;
    }
}

//switch position initialization
function detectColorScheme() {
    var theme = "light";    //default to light

    //local storage is used to override OS theme settings
    if (localStorage.getItem("theme")) {
        if (localStorage.getItem("theme") == "dark") {
            toggleSwitch.checked = true;
        }
    } else if (!window.matchMedia) {
        //matchMedia method not supported
        return false;
    } else if (window.matchMedia("(prefers-color-scheme: dark)").matches) {
        //OS theme setting detected as dark
        toggleSwitch.checked = true;
    }
}
detectColorScheme();