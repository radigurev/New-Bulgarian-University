#include <iostream>
#include <string>
using namespace std;

// Структура за възел в ДДП (BST)
struct TreeNode {
    int value;
    TreeNode* left;
    TreeNode* right;

    TreeNode(int v) : value(v), left(nullptr), right(nullptr) {}
};

// Функция за вмъкване на стойност в BST
TreeNode* insertBST(TreeNode* root, int val) {
    if (root == nullptr) {
        // Ако дървото е празно, създаваме нов възел
        return new TreeNode(val);
    }
    if (val < root->value) {
        // Вмъкваме в ляво поддърво
        root->left = insertBST(root->left, val);
    } else if (val > root->value) {
        // Вмъкваме в дясно поддърво
        root->right = insertBST(root->right, val);
    } else {
        // При равни стойности - по условие може да не вмъкваме повторно,
        // или да решим друго поведение. Тук просто го игнорираме.
    }
    return root;
}

// Намираме минималната стойност в дадено поддърво (ползва се при изтриване).
TreeNode* findMin(TreeNode* root) {
    while (root && root->left != nullptr) {
        root = root->left;
    }
    return root;
}

// Изтриване на възел от BST. Връща (възможно нов) корен на поддървото.
TreeNode* deleteBST(TreeNode* root, int val) {
    if (root == nullptr) {
        return nullptr; // няма такъв елемент за триене
    }
    if (val < root->value) {
        // Търсим за триене в лявото поддърво
        root->left = deleteBST(root->left, val);
    }
    else if (val > root->value) {
        // Търсим за триене в дясното поддърво
        root->right = deleteBST(root->right, val);
    }
    else {
        // Намерили сме възела за изтриване
        if (root->left == nullptr && root->right == nullptr) {
            // 1) Лист - изтриваме директно
            delete root;
            root = nullptr;
        }
        else if (root->left == nullptr) {
            // 2) Има само дясно дете
            TreeNode* temp = root->right;
            delete root;
            root = temp;
        }
        else if (root->right == nullptr) {
            // 2) Има само ляво дете
            TreeNode* temp = root->left;
            delete root;
            root = temp;
        }
        else {
            // 3) Има две деца
            // Намираме най-малкия във дясното поддърво
            TreeNode* minNode = findMin(root->right);
            // Копираме стойността му в текущия възел
            root->value = minNode->value;
            // Изтриваме вече пренесената стойност от дясното поддърво
            root->right = deleteBST(root->right, minNode->value);
        }
    }
    return root;
}

// Рекурсивно "графично" отпечатване на BST (наклонено).
int count1 = 0;
void printTree(TreeNode* root1){
    if(root1){
        cout << "\t" << root1->value;
        count1++;
        printTree(root1->right);
        count1--;
        cout << endl;
        for(int i=1; i<=count1; i++) cout <<"\t";
        count1++;
        printTree(root1->left);
        count1--;
    }
}

int main() {
    ios::sync_with_stdio(false);
    cin.tie(nullptr);

    // 1) Вмъкваме начални стойности в дървото (примерно N стойности).
    //    Може да ги прочетем от клавиатура,
    //    или директно "хардкодно" за демо, според заданието.
    int N;
    cout << "Въведете брой начални елементи N: ";
    cin >> N;

    TreeNode* root = nullptr;
    cout << "Въведете " << N << " стойности за вмъкване в BST:\n";
    for (int i = 0; i < N; i++) {
        int val;
        cin >> val;
        root = insertBST(root, val);
    }

    // Печатаме първоначално дърво
    cout << "\n=== Дърво след вмъкване на началните елементи ===\n";
    printTree(root);

    // 2) Четем колко стойности ще трием
    int M;
    cout << "\nВъведете брой стойности за триене M: ";
    cin >> M;

    cout << "Въведете " << M << " стойности (една по една или на един ред):\n";
    for (int i = 0; i < M; i++) {
        int valDel;
        cin >> valDel;
        // Трием стойността
        root = deleteBST(root, valDel);

        // Печатаме дървото след всяко триене
        cout << "\nДървото след триене на " << valDel << ":\n";
        printTree(root);
    }

    return 0;
}
