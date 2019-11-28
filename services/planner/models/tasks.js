'use strict';
const orm = require('nodeOrm');
const Model = orm.Model;


module.exports = new Model('tasks', {
    planner: Number,
    name: String,
    description: String,
    date: String,
    time: String
});
