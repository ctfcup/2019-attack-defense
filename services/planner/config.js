'use strict';
const usersModel = require('./models/users');
const plannersModel = require('./models/planners');
const tasksModel = require('./models/tasks');

module.exports.db = {
    host:'127.0.0.7', 
    user: 'planner', 
    password: 'planner',
    database:'db', 
    tables: [usersModel, plannersModel, tasksModel]
}

module.exports.service = {
    port: 1337
}

