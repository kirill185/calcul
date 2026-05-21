import json
import os
from tkinter import *
from tkinter import ttk, messagebox, simpledialog

class TestSystem:
    def __init__(self, root):
        self.root = root
        self.root.title("Тесты")
        self.root.geometry("1000x700")
        self.root.configure(bg='#1e1e1e')
        
        self.data_file = "tests.json"
        self.tests = self.load_tests()
        self.current_test = None
        self.user_answers = {}
        self.current_q_index = 0
        
        self.setup_ui()
        self.refresh_tests()
    
    def load_tests(self):
        if os.path.exists(self.data_file):
            try:
                with open(self.data_file, 'r', encoding='utf-8') as f:
                    return json.load(f)
            except:
                return {}
        return {}
    
    def save_tests(self):
        with open(self.data_file, 'w', encoding='utf-8') as f:
            json.dump(self.tests, f, ensure_ascii=False, indent=2)
    
    def setup_ui(self):
        main_frame = Frame(self.root, bg='#1e1e1e')
        main_frame.pack(fill=BOTH, expand=True, padx=10, pady=10)
        
        left_frame = LabelFrame(main_frame, text="Тесты", fg='white', bg='#2d2d2d', font=('Arial', 10, 'bold'))
        left_frame.pack(side=LEFT, fill=BOTH, expand=True, padx=(0,5))
        
        self.test_list = Listbox(left_frame, bg='#2d2d2d', fg='white', selectbackground='#007acc',
                                 font=('Arial', 11), bd=0, highlightthickness=0)
        self.test_list.pack(fill=BOTH, expand=True, padx=5, pady=5)
        self.test_list.bind('<<ListboxSelect>>', self.on_test_select)
        
        right_frame = LabelFrame(main_frame, text="Редактор", fg='white', bg='#2d2d2d', font=('Arial', 10, 'bold'))
        right_frame.pack(side=RIGHT, fill=BOTH, expand=True, padx=(5,0))
        
        btn_frame = Frame(right_frame, bg='#2d2d2d')
        btn_frame.pack(fill=X, padx=5, pady=5)
        
        buttons = [
            ("Добавить тест", self.add_test),
            ("Удалить тест", self.delete_test),
            ("Все вопросы", self.show_all),
            ("Пройти тест", self.start_test)
        ]
        
        for text, cmd in buttons:
            btn = Button(btn_frame, text=text, command=cmd, bg='#007acc', fg='white',
                        font=('Arial', 9), cursor='hand2', bd=0, padx=10, pady=5)
            btn.pack(side=LEFT, padx=2)
        
        self.test_label = Label(right_frame, text="Тест не выбран", fg='#007acc', bg='#2d2d2d',
                               font=('Arial', 12, 'bold'))
        self.test_label.pack(pady=5)
        
        q_frame = LabelFrame(right_frame, text="Вопросы", fg='white', bg='#2d2d2d')
        q_frame.pack(fill=BOTH, expand=True, padx=5, pady=5)
        
        self.q_list = Listbox(q_frame, bg='#2d2d2d', fg='white', selectbackground='#007acc',
                             font=('Arial', 10), height=8)
        self.q_list.pack(fill=BOTH, expand=True, padx=5, pady=5)
        self.q_list.bind('<<ListboxSelect>>', self.on_q_select)
        
        edit_frame = Frame(right_frame, bg='#2d2d2d')
        edit_frame.pack(fill=X, padx=5, pady=5)
        
        Label(edit_frame, text="Вопрос:", fg='white', bg='#2d2d2d', font=('Arial', 9)).pack(anchor=W)
        self.q_text = Text(edit_frame, height=3, bg='#1e1e1e', fg='white', insertbackground='white',
                          font=('Arial', 10))
        self.q_text.pack(fill=X, pady=(0,5))
        
        Label(edit_frame, text="Ответ:", fg='white', bg='#2d2d2d', font=('Arial', 9)).pack(anchor=W)
        self.answer_text = Entry(edit_frame, bg='#1e1e1e', fg='white', insertbackground='white',
                                font=('Arial', 10))
        self.answer_text.pack(fill=X, pady=(0,5))
        
        btn_frame2 = Frame(edit_frame, bg='#2d2d2d')
        btn_frame2.pack(fill=X, pady=5)
        
        ed_buttons = [
            ("Добавить", self.add_q),
            ("Обновить", self.update_q),
            ("Удалить", self.delete_q),
            ("Очистить", self.clear_form)
        ]
        
        for text, cmd in ed_buttons:
            btn = Button(btn_frame2, text=text, command=cmd, bg='#3c3c3c', fg='white',
                        font=('Arial', 9), cursor='hand2', bd=0, padx=10, pady=5)
            btn.pack(side=LEFT, padx=2, expand=True, fill=X)
    
    def refresh_tests(self):
        self.test_list.delete(0, END)
        for name in self.tests:
            self.test_list.insert(END, name)
        if not self.tests:
            self.test_list.insert(END, "Нет тестов")
    
    def refresh_q_list(self):
        self.q_list.delete(0, END)
        if self.current_test and self.current_test in self.tests:
            for i, q in enumerate(self.tests[self.current_test], 1):
                text = q['question'][:50] + "..." if len(q['question']) > 50 else q['question']
                self.q_list.insert(END, f"{i}. {text}")
    
    def on_test_select(self, e):
        sel = self.test_list.curselection()
        if sel and self.tests:
            name = self.test_list.get(sel[0])
            if name != "Нет тестов":
                self.current_test = name
                self.test_label.config(text=self.current_test)
                self.refresh_q_list()
                self.clear_form()
    
    def on_q_select(self, e):
        sel = self.q_list.curselection()
        if sel and self.current_test:
            idx = sel[0]
            if idx < len(self.tests[self.current_test]):
                q = self.tests[self.current_test][idx]
                self.q_text.delete(1.0, END)
                self.q_text.insert(1.0, q['question'])
                self.answer_text.delete(0, END)
                self.answer_text.insert(0, q['answer'])
    
    def add_test(self):
        name = simpledialog.askstring("Новый тест", "Название:", parent=self.root)
        if name:
            if name in self.tests:
                messagebox.showerror("Ошибка", "Такой тест есть")
                return
            self.tests[name] = []
            self.save_tests()
            self.refresh_tests()
            messagebox.showinfo("Готово", f"Тест {name} создан")
    
    def delete_test(self):
        if not self.current_test:
            messagebox.showwarning("Ошибка", "Выберите тест")
            return
        if messagebox.askyesno("Удалить", f"Удалить {self.current_test}?"):
            del self.tests[self.current_test]
            self.save_tests()
            self.current_test = None
            self.test_label.config(text="Тест не выбран")
            self.refresh_tests()
            self.refresh_q_list()
            self.clear_form()
    
    def add_q(self):
        if not self.current_test:
            messagebox.showwarning("Ошибка", "Выберите тест")
            return
        q = self.q_text.get(1.0, END).strip()
        a = self.answer_text.get().strip()
        if not q or not a:
            messagebox.showwarning("Ошибка", "Заполните поля")
            return
        self.tests[self.current_test].append({'question': q, 'answer': a})
        self.save_tests()
        self.refresh_q_list()
        self.clear_form()
    
    def update_q(self):
        if not self.current_test:
            messagebox.showwarning("Ошибка", "Выберите тест")
            return
        sel = self.q_list.curselection()
        if not sel:
            messagebox.showwarning("Ошибка", "Выберите вопрос")
            return
        q = self.q_text.get(1.0, END).strip()
        a = self.answer_text.get().strip()
        if not q or not a:
            messagebox.showwarning("Ошибка", "Заполните поля")
            return
        self.tests[self.current_test][sel[0]] = {'question': q, 'answer': a}
        self.save_tests()
        self.refresh_q_list()
        self.clear_form()
    
    def delete_q(self):
        if not self.current_test:
            messagebox.showwarning("Ошибка", "Выберите тест")
            return
        sel = self.q_list.curselection()
        if not sel:
            messagebox.showwarning("Ошибка", "Выберите вопрос")
            return
        if messagebox.askyesno("Удалить", "Удалить вопрос?"):
            del self.tests[self.current_test][sel[0]]
            self.save_tests()
            self.refresh_q_list()
            self.clear_form()
    
    def clear_form(self):
        self.q_text.delete(1.0, END)
        self.answer_text.delete(0, END)
        self.q_list.selection_clear(0, END)
    
    def show_all(self):
        if not self.current_test:
            messagebox.showwarning("Ошибка", "Выберите тест")
            return
        if not self.tests[self.current_test]:
            messagebox.showinfo("Инфо", "Нет вопросов")
            return
        
        win = Toplevel(self.root)
        win.title(f"{self.current_test} - все вопросы")
        win.geometry("700x500")
        win.configure(bg='#1e1e1e')
        
        text = Text(win, bg='#2d2d2d', fg='white', font=('Arial', 11), wrap=WORD)
        text.pack(fill=BOTH, expand=True, padx=10, pady=10)
        
        scroll = Scrollbar(text, command=text.yview)
        text.configure(yscrollcommand=scroll.set)
        scroll.pack(side=RIGHT, fill=Y)
        
        content = f"Тест: {self.current_test}\n{'-'*50}\n\n"
        for i, q in enumerate(self.tests[self.current_test], 1):
            content += f"{i}. {q['question']}\n"
            content += f"   Ответ: {q['answer']}\n\n"
        
        text.insert(1.0, content)
        text.config(state=DISABLED)
        
        Button(win, text="Закрыть", command=win.destroy, bg='#007acc', fg='white', bd=0, padx=20, pady=5).pack(pady=10)
    
    def start_test(self):
        if not self.current_test:
            messagebox.showwarning("Ошибка", "Выберите тест")
            return
        if not self.tests[self.current_test]:
            messagebox.showwarning("Ошибка", "Нет вопросов")
            return
        
        self.user_answers = {}
        self.current_q_index = 0
        
        self.test_win = Toplevel(self.root)
        self.test_win.title(f"Тест: {self.current_test}")
        self.test_win.geometry("700x500")
        self.test_win.configure(bg='#1e1e1e')
        
        self.show_question()
    
    def show_question(self):
        for w in self.test_win.winfo_children():
            w.destroy()
        
        total = len(self.tests[self.current_test])
        q_data = self.tests[self.current_test][self.current_q_index]
        
        header = Frame(self.test_win, bg='#2d2d2d', height=50)
        header.pack(fill=X)
        Label(header, text=f"{self.current_q_index + 1} / {total}", 
              bg='#2d2d2d', fg='#007acc', font=('Arial', 14, 'bold')).pack(pady=10)
        
        main = Frame(self.test_win, bg='#1e1e1e')
        main.pack(fill=BOTH, expand=True, padx=20, pady=20)
        
        q_frame = Frame(main, bg='#2d2d2d')
        q_frame.pack(fill=BOTH, expand=True)
        Label(q_frame, text=q_data['question'], bg='#2d2d2d', fg='white', 
              font=('Arial', 12), wraplength=600, justify=LEFT).pack(padx=20, pady=30)
        
        a_frame = Frame(main, bg='#1e1e1e')
        a_frame.pack(fill=X, pady=10)
        Label(a_frame, text="Ваш ответ:", bg='#1e1e1e', fg='white', font=('Arial', 10)).pack(anchor=W)
        
        self.test_answer = Entry(a_frame, bg='#2d2d2d', fg='white', font=('Arial', 11), 
                                 insertbackground='white')
        self.test_answer.pack(fill=X, pady=5, ipady=5)
        
        if self.current_q_index in self.user_answers:
            self.test_answer.insert(0, self.user_answers[self.current_q_index])
        
        btn_frame = Frame(main, bg='#1e1e1e')
        btn_frame.pack(fill=X, pady=20)
        
        if self.current_q_index > 0:
            Button(btn_frame, text="Назад", command=self.prev_q, bg='#3c3c3c', fg='white', bd=0, padx=15, pady=5).pack(side=LEFT, padx=5)
        
        if self.current_q_index < total - 1:
            Button(btn_frame, text="Далее", command=self.next_q, bg='#007acc', fg='white', bd=0, padx=15, pady=5).pack(side=RIGHT, padx=5)
        else:
            Button(btn_frame, text="Завершить", command=self.finish_test, bg='#007acc', fg='white', bd=0, padx=15, pady=5).pack(side=RIGHT, padx=5)
    
    def save_current(self):
        if hasattr(self, 'test_answer'):
            ans = self.test_answer.get().strip()
            if ans:
                self.user_answers[self.current_q_index] = ans
    
    def next_q(self):
        self.save_current()
        self.current_q_index += 1
        self.show_question()
    
    def prev_q(self):
        self.save_current()
        self.current_q_index -= 1
        self.show_question()
    
    def finish_test(self):
        self.save_current()
        
        correct = 0
        total = len(self.tests[self.current_test])
        
        for i, q in enumerate(self.tests[self.current_test]):
            user = self.user_answers.get(i, "").strip().lower()
            right = q['answer'].strip().lower()
            if user == right:
                correct += 1
        
        percent = (correct / total) * 100
        
        result_win = Toplevel(self.test_win)
        result_win.title("Результаты")
        result_win.geometry("600x500")
        result_win.configure(bg='#1e1e1e')
        
        text = Text(result_win, bg='#2d2d2d', fg='white', font=('Arial', 11), wrap=WORD)
        text.pack(fill=BOTH, expand=True, padx=10, pady=10)
        
        scroll = Scrollbar(text, command=text.yview)
        text.configure(yscrollcommand=scroll.set)
        scroll.pack(side=RIGHT, fill=Y)
        
        res = f"Результат: {correct} / {total} ({percent:.0f}%)\n\n"
        
        if percent >= 80:
            res += "Оценка: Отлично!\n\n"
        elif percent >= 60:
            res += "Оценка: Хорошо\n\n"
        elif percent >= 40:
            res += "Оценка: Удовлетворительно\n\n"
        else:
            res += "Оценка: Плохо\n\n"
        
        res += "-"*50 + "\n\nРазбор:\n\n"
        
        for i, q in enumerate(self.tests[self.current_test]):
            user = self.user_answers.get(i, "Нет ответа")
            right = q['answer']
            status = "✓" if user.strip().lower() == right.strip().lower() else "✗"
            res += f"{status} {q['question']}\n"
            res += f"   Твой ответ: {user}\n"
            res += f"   Правильно: {right}\n\n"
        
        text.insert(1.0, res)
        text.config(state=DISABLED)
        
        Button(result_win, text="Закрыть", command=lambda: [result_win.destroy(), self.test_win.destroy()], 
               bg='#007acc', fg='white', bd=0, padx=20, pady=5).pack(pady=10)

if __name__ == "__main__":
    root = Tk()
    app = TestSystem(root)
    root.mainloop()