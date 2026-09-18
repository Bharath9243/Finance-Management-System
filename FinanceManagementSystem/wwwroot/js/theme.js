window.themeManager = {
    initialize: function () {
        const savedTheme = localStorage.getItem("theme");

        if (savedTheme === "dark") {
            document.documentElement.setAttribute("data-theme", "dark");
        } else {
            document.documentElement.removeAttribute("data-theme");
        }
    },

    toggle: function () {
        const html = document.documentElement;
        const isDark = html.getAttribute("data-theme") === "dark";

        if (isDark) {
            html.removeAttribute("data-theme");
            localStorage.setItem("theme", "light");
            return false;
        }

        html.setAttribute("data-theme", "dark");
        localStorage.setItem("theme", "dark");
        return true;
    }
};

