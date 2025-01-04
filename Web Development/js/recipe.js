import { FindElement } from './jsonHandler.js'

const categoryParameter = GetFilter();
const recipe = await FindElement('recipes', categoryParameter);

SetBackground();

const PopulateRecipePage = () => {
    const recipeIngrediants = document.getElementById('recipe-ingrediants');
    const title = document.getElementById('title');
    const recipeText = document.getElementById('recipe-text');
    const recipeImage = document.getElementById('recipe-image');

    title.innerText = recipe.name;
    recipeImage.src = recipe.image;
    recipeText.innerHTML = `<span>${recipe.recipeSteps.replace(/\n/g, '<br>')}</span>`;

    let ingredientsHtml = '';
    recipe.recipeIngredients.forEach((ingredient, i) => {
        ingredientsHtml += `<p>${i+1}. ${ingredient}</p>`;
      });  

    recipeIngrediants.innerHTML = ingredientsHtml;
}

PopulateRecipePage();

function GetFilter() {
    let urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('recipeid');
}

async function SetBackground() {
    if (IsCategoryParameterInvalid) return;

    if (recipe) document.body.style.backgroundImage = `url(${cuisine.image})`;
    else console.log("Cuisine not found");
}

function IsCategoryParameterInvalid() {
    return categoryParameter === null || isNaN(categoryParameter);
}