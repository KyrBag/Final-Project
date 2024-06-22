/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
  ],
  // darkMode: "class",
  theme: {
    // screens: {
    //   sm: "480px",
    //   md: "768px",
    //   lg: "976px",
    //   xl: "1400px",
    extend: {

      fontFamily:{
        poppins: ["Poppins", "sans-serif"]
      }
    },
  },
  plugins: [],
}
