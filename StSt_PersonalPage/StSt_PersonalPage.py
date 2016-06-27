from flask import Flask,  url_for, abort, request, Response, render_template, redirect, session, jsonify

import db_interaction

app = Flask(__name__)
app.secret_key = 'whoknowsthissecretw'


@app.route('/')
def index():
    return redirect(url_for('login'))


@app.route('/login.html')
def login():
    return render_template('login.html')

@app.route('/redirect', methods=['POST'])
def redirecting():
        user = request.form['username']
        if db_interaction.get_session(user).__len__()>0:
            session_list = db_interaction.get_session(user)
            session['user'] = session_list
            return redirect(url_for('get_sessions', user=user))
        else:
            return 'noUser.html'

@app.route('/sessions/<user>', methods=['GET'])
def get_sessions(user):
    return render_template('PersonalPage.html', name=user)


@app.route('/sessions/json/<user>', methods=['GET'])
def get_sessions_json(user):
    sessions =[]
    items =db_interaction.get_session(user)
    for item in items:
        sessions.append(prepare_for_json(item))
    return jsonify({'sessions': sessions})


@app.route('/sessions', methods=['POST'])
def insert_session():

    # get the request body
    print(request)
    add_request = request.json

    # check whether a task is present in the request or not
    if add_request is not None and ('start' and 'end' and 'day' and 'position' and 'light' and 'name') in add_request:
        name = add_request['name']
        start = add_request['start']
        end = add_request['stop']
        day = add_request['day']
        position = add_request['position']
        light = add_request['light']

        # insert in the database
        start1 = day + " "+ start
        end1 = day + " " + end
        db_interaction.insert_session(name, start1, end1, position, light)

        return Response(status=200)

    # return an error in case of problems
    abort(403)


def prepare_for_json(item):
    """
    Convert the task in a dictionary for easing the JSON creation
    """
    session = dict()
    session['name'] = item[0]
    session['start'] = item[1]
    session['end'] = item[2]
    session['position'] = item[3]
    session['light'] = item[4]
    return session

if __name__ == '__main__':
    app.run(debug=True)
