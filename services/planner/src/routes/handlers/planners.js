var router = require('express').Router();

router.get('/', async function(req, resp, next) {
    const res = await req.app.get('orm')
                        .tables.users.filter({
                            id: req.authedUserId
                        });
    
    if (res.length == 0)
        resp.sendStatus(404);
    const user = res[0];
    const planners = await req.app.get('orm')
                        .tables.planners.filter({
                            owner: req.authedUserId
                        });
    resp.render('planners.html', {isAuthed: true, planners: planners});
});

router.post('/', async function(req, resp, next) {
    const res = await req.app.get('orm')
                        .tables.users.filter({
                            id: req.authedUserId
                        });
    if (res.length == 0)
        resp.sendStatus(404);
    const user = res[0];
    let id = await req.app.get('orm')
                    .tables.planners.add({
                        owner: user.id, 
                        name: req.body.name, 
                        description: req.body.description,
                        creationTime: req.body.creationTime
                    });
    resp.status(201).json({'id':id});
});

router.get('/:planner_id', async function(req, resp, next) {    
    let planner = await req.app.get('orm')
                        .tables.planners.filter({
                            id: parseInt(req.params.planner_id), owner: req.authedUserId
                        });
    if (planner.length == 0)
        resp.sendStatus(404);
    
    let hours = [];
    for (let hour=6; hour < 23; hour+=2)
        hours.push(`${String(hour).padStart(2,"0")}:00`);
    let week = ['Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday', 'Sunday'];
    
    resp.render('planner.html', {isAuthed: true, planner_name: planner[0].name, hours: hours, 
                                    week_days: week});
});

router.post('/:planner_id', async function(req, resp, next) {
    const planner = await req.app.get('orm')
                        .tables.planners.filter({
                            id: parseInt(req.params.planner_id), owner: req.authedUserId
                        });
    if (planner.length == 0)
        resp.sendStatus(404);
    console.log("ADD TASK");
    console.log(req.body);
    
    let id = await req.app.get('orm')
                    .tables.tasks.add({
                        planner: planner[0].id, 
                        name: req.body.name, 
                        description: req.body.description,
                        year: req.body.year,
                        week: req.body.week,
                        day: req.body.day,
                        time: req.body.time
                    });
    resp.sendStatus(201);
});


module.exports = router;