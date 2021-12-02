
function activeNav(x) {

    if (document.getElementById("Home").classList.contains('active')) {
        document.getElementById("Home").classList.remove('active');
    }

    if (document.getElementById("Java").classList.contains('active')) {
        document.getElementById("Java").classList.remove('active');
    }

    if (document.getElementById("net").classList.contains('active')) {
        document.getElementById("net").classList.remove('active');
    }

    if (document.getElementById("Python").classList.contains('active')) {
        document.getElementById("Python").classList.remove('active');
    }

    if (x == 0) {
        document.getElementById("Home").classList.add('active');
    } else if (x == 1) {
        document.getElementById("Java").classList.add('active');
    } else if (x == 2) {
        document.getElementById("net").classList.add('active');
    } else if (x == 3) {
        document.getElementById("Python").classList.add('active');
    }
}