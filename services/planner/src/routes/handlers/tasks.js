var router = require('express').Router();

router.get('/', async function(req, resp, next) {
    let planner_id = 0;
    try {
        planner_id = parseInt(req.baseUrl.split('/')[2]);
        week = parseInt(req.query.week);
        year = parseInt(req.query.year);
    }
    catch(err) {
        resp.sendStatus(404);
    }

    let planner = await req.app.get('orm')
                        .tables.planners.filter({
                            id: planner_id, owner: req.authedUserId
                        });
    if (planner.length == 0)
        resp.sendStatus(404);
    planner = planner[0];

    let tasks = {};
    if (isNaN(week) || isNaN(year))
        resp.sendStatus(404);
    else
        tasks = await req.app.get('orm').tables.tasks.filter({
                                planner: planner.id, week: week, year: year
                            });
    resp.json(tasks);
});

module.exports = router;