import sqlite3

conn = sqlite3.connect('Stst')
c = conn.cursor()


def insert_session(name, start, end, position, light):
    """
    Insert a task in the database
    """

    # prepare the query
    sql = """INSERT INTO sessions(name, start, end, position, light) VALUES ( ?, ?, ?, ?, ?)"""

    # connect to the db
    conn = sqlite3.connect("/home/gabri/Documenti/Pycharm_projects/StSt_PersonalPage/Stst")
    cursor = conn.cursor()

    try:
        # execute the query passing the needed parameters
        cursor.execute(sql, (name, start, end, position, light))
        # commit all pending queries
        conn.commit()
    except Exception, e:
        print str(e)
        # if something goes wrong: rollback
        conn.rollback()

    # close the connection
    conn.close()


def get_sessions():
    """
    Get existing tasks from the database
    """

    sessions = [ ]
    sql = "SELECT name, start, end, position, light  FROM sessions"
    conn = sqlite3.connect("/home/gabri/Documenti/Pycharm_projects/StSt_PersonalPage/Stst")

    # to remove u from sqlite3 cursor.fetchall() results
    conn.text_factory = sqlite3.OptimizedUnicode

    cursor = conn.cursor()
    cursor.execute(sql)

    results = cursor.fetchall()

    for session in results:
        sessions.append(session)

    conn.close()

    return sessions


def get_session(name):


    # prepare the query
    """

    :rtype: object
    """
    sql = "SELECT name, start, end, position, light FROM sessions WHERE name = ?"

    # connect to the db
    conn = sqlite3.connect("/home/gabri/Documenti/Pycharm_projects/StSt_PersonalPage/Stst")

    # to remove u from sqlite3 cursor.fetchall() results
    conn.text_factory = sqlite3.OptimizedUnicode

    cursor = conn.cursor()
    cursor.execute(sql, (name,))

    results = cursor.fetchall()
    print results

    sessions = []

    for session in results:
        sessions.append(session)

    # close the connection
    conn.close()

    return sessions


def remove_task_by_id(name):
    """
    :param name: unique identifier for the task we want to remove
    Remove a specific task from the db
    """

    # prepare the query
    sql = "DELETE FROM session WHERE name = ?"

    # connect to the db
    conn = sqlite3.connect("Stst")
    cursor = conn.cursor()

    try:
        # execute the query passing the needed parameters
        cursor.execute(sql, (name,))
        # commit all pending executed queries in the connection
        conn.commit()
    except Exception, e:
        print str(e)
        # if something goes wrong: rollback
        conn.rollback()

    # close the connection
    conn.close()


if __name__ == "__main__":
    print(get_sessions())
