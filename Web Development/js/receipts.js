import { PopulatePage, FindElement } from './jsonHandler.js';
import { CreateCard } from './CardPainter.js';

const articlesContainer = document.getElementById('articles');
const categoryParameter = GetFilter();
SetBackground();

const PopulateCuisines = (data) => data.forEach(recipe => CreateRecipeCard(recipe));
PopulatePage('recipes', PopulateCuisines);

function CreateRecipeCard(recipe) {
    if (IsCategoryParameterInvalid()) {
        console.log('InvalidParameter: ' + categoryParameter);
        CreateCard(recipe, articlesContainer, 'ViewRecipe', '');
        return;
    }

    if (recipe.cuisine == categoryParameter) CreateCard(recipe, articlesContainer, 'ViewRecipe', `recipeid=${recipe.id}`);
}

function GetFilter() {
    let urlParams = new URLSearchParams(window.location.search);
    return urlParams.get('categoryid');
}

async function SetBackground() {
    if (IsCategoryParameterInvalid) return;

    let cuisine = await FindElement('cuisines', categoryParameter);

    if (cuisine) document.body.style.backgroundImage = `url(${cuisine.image})`;
    else console.log("Cuisine not found");
}

function IsCategoryParameterInvalid() {
    return categoryParameter === null || isNaN(categoryParameter);
}