import sys

problems = {
    "p0": {
        "code": '''
string = "t9a52d6am"
{user_code}
''',
        "var": "secret_code",
        "expected": "tadam"
    },
    "p1": {
        "code": '''
chest_locked = True
has_key_code = False
{user_code}
if has_key_code:
    chest_locked = False
''',
        "var": "has_key_code",
        "expected": "True"
    },
    "p2": {
        "code": '''
first_number = 4
second_number = 5
third_number = 10
button_pressed = True
code = first_number * second_number + third_number
answer = {user_code}
if answer == code and button_pressed == True:
    door_open = True
else:
    door_open = False
''',
        "var": "answer",
        "expected": "30"
    },
    "p3": {
        "code": '''
servers_status = [True, False, True, True, False]
online_count = 0
for status in servers_status:
    if {user_code}:
        online_count += 1
''',
        "var": "online_count",
        "expected": "3"
    },
    "p4": {
        "code": '''
passwords = [8271, 1235, 4312, 9001]
i = 0
password = 1
while i < 4:
    if passwords[i] % 2 == {user_code}:
        password = passwords[i]
    i += 1
if password == 4312:
    activate_robot = lambda: None
    activate_robot()
''',
        "var": "password",
        "expected": "4312"
    },
    "p5": {
        "code": '''
first_sensor = [110, 230, 450, 670]
second_sensor = [120, 240, 460, 680]
log = []
for i in range(4):
    log.append(first_sensor[i])
    {user_code}
''',
        "var": "log",
        "expected": "[110, 120, 230, 240, 450, 460, 670, 680]"
    },
    "p6": {
        "code": '''
nums = [3, 2, 4]
target = 6
answer = None
for i in range(len(nums)):
    for j in range(i + 1, len(nums)):
        if {user_code}:
            answer = [i, j]
if answer == [1, 2]:
    unlock_final_door = lambda: None
    unlock_final_door()
''',
        "var": "answer",
        "expected": "[1, 2]"
    }
}

if len(sys.argv) != 3:
    print("wrong")
    sys.exit(1)

problem_id = sys.argv[1]
user_code_input = sys.argv[2]

if problem_id not in problems:
    print("wrong")
    sys.exit(1)

problem = problems[problem_id]
formatted_code = problem["code"].replace("{user_code}", user_code_input)
expected_value = str(problem["expected"])
var_name = problem["var"]

local_env = {}
try:
    exec(formatted_code, {}, local_env)
    actual_value = str(local_env.get(var_name))
    if actual_value == expected_value:
        print("correct")
    else:
        print("wrong")
except Exception:
    print("wrong")
