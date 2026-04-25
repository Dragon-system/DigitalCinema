$(document).ready(function () {
  $(".movie-carousel").owlCarousel({
    loop: true,
    margin: 15,
    nav: true,
    autoplay: true,
    autoplayTimeout: 3000,
    autoplayHoverPause: true,
    responsive: {
      0: { items: 1 },
      480: { items: 2 },
      768: { items: 3 },
      1000: { items: 5 },
    },
  });
});
