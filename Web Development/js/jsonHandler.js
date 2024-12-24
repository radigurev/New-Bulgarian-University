export const PopulatePage = (json, callBack) => fetch(`../json/${json}.json`)
                                      .then(response => response.json())
                                      .then(data => callBack(data));
