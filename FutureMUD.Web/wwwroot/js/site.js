(() => {
	const reducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)");
	for (const carousel of document.querySelectorAll("[data-carousel]")) {
		const viewport = carousel.querySelector("[data-carousel-viewport]");
		const slides = Array.from(carousel.querySelectorAll("[data-carousel-slide]"));
		const previous = carousel.querySelector("[data-carousel-previous]");
		const next = carousel.querySelector("[data-carousel-next]");
		const status = carousel.querySelector("[data-carousel-status]");
		if (!viewport || !previous || !next || !status || slides.length < 2) {
			continue;
		}

		let current = 0;
		let scrollFrame = 0;
		const slideLeft = (slide) => slide.offsetLeft - viewport.offsetLeft;
		const updateStatus = (index) => {
			current = index;
			status.textContent = `${index + 1} of ${slides.length}`;
			slides.forEach((slide, slideIndex) => {
				if (slideIndex === index) {
					slide.setAttribute("aria-current", "true");
				} else {
					slide.removeAttribute("aria-current");
				}
			});
		};
		const showSlide = (index) => {
			const target = (index + slides.length) % slides.length;
			viewport.scrollTo({
				left: slideLeft(slides[target]),
				behavior: reducedMotion.matches ? "auto" : "smooth"
			});
			updateStatus(target);
		};

		previous.addEventListener("click", () => showSlide(current - 1));
		next.addEventListener("click", () => showSlide(current + 1));
		viewport.addEventListener("keydown", (event) => {
			if (event.key === "ArrowLeft" || event.key === "ArrowRight") {
				event.preventDefault();
				showSlide(current + (event.key === "ArrowLeft" ? -1 : 1));
			}
		});
		viewport.addEventListener("scroll", () => {
			cancelAnimationFrame(scrollFrame);
			scrollFrame = requestAnimationFrame(() => {
				let nearest = 0;
				let nearestDistance = Number.POSITIVE_INFINITY;
				slides.forEach((slide, index) => {
					const distance = Math.abs(slideLeft(slide) - viewport.scrollLeft);
					if (distance < nearestDistance) {
						nearest = index;
						nearestDistance = distance;
					}
				});
				if (nearest !== current) {
					updateStatus(nearest);
				}
			});
		}, { passive: true });

		carousel.classList.add("carousel-ready");
		updateStatus(0);
	}
})();
