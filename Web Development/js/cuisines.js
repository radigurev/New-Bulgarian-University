import { PopulatePage } from './jsonHandler.js';
import { CreateCard } from './CardPainter.js';

const articlesContainer = document.getElementById('articles');

const PopulateCuisines = (data) => data.forEach(cuisine => CreateCard(cuisine, articlesContainer, 'ViewRecipies', `categoryid=${cuisine.id}`));
PopulatePage('cuisines', PopulateCuisines);