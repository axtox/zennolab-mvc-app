let form = document.querySelector('form');

let nameInput = form.querySelector('input');
let checkBoxes = form.querySelectorAll("input[type=checkbox]");

form.addEventListener('submit', event => {
    let validationPassed = false;

    //one of first 3 checkboxes must be checked
    checkBoxes.forEach(checkbox => {
        validationPassed = checkbox.checked;
    });

    //check if name field does not contain "captcha" word
    validationPassed &&= !nameInput.value.toLowerCase().includes("captcha");

    validationPassed &&= /^[a-zA-Z]+$/.test(nameInput.value)

    if(validationPassed)
        return;
    
    event.preventDefault();
});








nameInput.addEventListener('input', () => {
  nameInput.setCustomValidity('');
  nameInput.checkValidity();
});

nameInput.addEventListener('invalid', () => {
  if(nameInput.value === '') {
    nameInput.setCustomValidity('Enter your username!');
  } else {
    nameInput.setCustomValidity('Usernames can only contain upper and lowercase letters. Try again!');
  }
});