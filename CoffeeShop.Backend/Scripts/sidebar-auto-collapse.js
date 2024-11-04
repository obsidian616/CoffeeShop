
document.querySelectorAll('.nav-link[data-bs-toggle="collapse"]').forEach((link) => {
    link.addEventListener("click", function () {
        const currentCollapse = this.nextElementSibling;

        document.querySelectorAll(".collapse.show").forEach((collapse) => {
            if (collapse !== currentCollapse && !collapse.contains(currentCollapse)) {
                new bootstrap.Collapse(collapse, { toggle: false }).hide();
            }
        });
    });
});

document.querySelector(".sidebar").addEventListener("mouseleave", function () {
    setTimeout(() => {
        document.querySelectorAll(".collapse.show").forEach((collapse) => {
            new bootstrap.Collapse(collapse, { toggle: false }).hide();
        });
    }, 400);
});

document.querySelector('.sidebar').addEventListener('mouseenter', function () {
    document.querySelector('.content').style.marginLeft = '230px';
});

document.querySelector('.sidebar').addEventListener('mouseleave', function () {
    document.querySelector('.content').style.marginLeft = '100px';
});

const sidebar = document.querySelector('.sidebar');
const content = document.querySelector('.content');


sidebar.addEventListener('mouseenter', () => {
    content.style.marginLeft = '230px';
});

sidebar.addEventListener('mouseleave', () => {
    content.style.marginLeft = '100px'; 
});