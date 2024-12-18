const stars = document.querySelectorAll(".stars i");
const ratingInput = document.getElementById("Rating"); // Câmpul input ascuns

stars.forEach((star, index1) => {
    // Efect la hover
    star.addEventListener("mouseover", () => {
        stars.forEach((star, index2) => {
            index1 >= index2
                ? (star.style.transform = "scale(1.2)")
                : (star.style.transform = "scale(1)");
        });
    });

    // Anulare hover
    star.addEventListener("mouseout", () => {
        stars.forEach(star => star.style.transform = "scale(1)");
    });

    // Click pentru selectare
    star.addEventListener("click", () => {
        stars.forEach((star, index2) => {
            index1 >= index2
                ? star.classList.add("activestar")
                : star.classList.remove("activestar");
        });
        ratingInput.value = index1 + 1; // Setează valoarea ratingului
        console.log("Rating selectat:", ratingInput.value); // Testare
    });
});
