'use strict';
const orm = require('node-fluent-orm');
const Model = orm.Model;


module.exports = new Model('planners', {
    owner: Number,
    name: String,
    description: String,
    creationTime: String
});
