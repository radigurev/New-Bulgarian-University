export const PopulatePage = (json, callBack) => fetch(`../json/${json}.json`)
    .then(response => response.json())
    .then(data => callBack(data));

export const FindElement = async (json, id) => {
    const response = await fetch(`../json/${json}.json`);
    const data = await response.json();
    return FilterData(data, id);
};

function FilterData(array, id) {
    return array.find(element => element.id == id) || null;
}
