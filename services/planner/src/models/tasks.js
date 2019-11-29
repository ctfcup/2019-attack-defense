'use strict';
const orm = require('nodeorm');
const Model = orm.Model;


module.exports = new Model('tasks', {
    planner: Number,
    name: String,
    description: String,
    year: Number,
    week: Number,
    day: String,
    time: String
});
