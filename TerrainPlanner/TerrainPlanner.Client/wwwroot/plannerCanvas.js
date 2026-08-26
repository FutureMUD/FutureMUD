const instances = new WeakMap();

function key(x, y) { return `${x}:${y}`; }

function resizeCanvas(state) {
	const rect = state.canvas.getBoundingClientRect();
	const ratio = window.devicePixelRatio || 1;
	const width = Math.max(1, Math.floor(rect.width * ratio));
	const height = Math.max(1, Math.floor(rect.height * ratio));
	if (state.canvas.width !== width || state.canvas.height !== height) {
		state.canvas.width = width;
		state.canvas.height = height;
	}
	state.ratio = ratio;
	render(state);
}

function screenPoint(state, event) {
	const rect = state.canvas.getBoundingClientRect();
	return { x: (event.clientX - rect.left) * state.ratio, y: (event.clientY - rect.top) * state.ratio };
}

function mapPoint(state, point) {
	const size = state.tileSize * state.zoom * state.ratio;
	const x = Math.floor((point.x - state.panX * state.ratio) / size);
	const displayY = Math.floor((point.y - state.panY * state.ratio) / size);
	const y = state.height - 1 - displayY;
	return x >= 0 && x < state.width && y >= 0 && y < state.height ? { x, y } : null;
}

function queueStroke(state, coordinate) {
	if (!coordinate) return;
	state.stroke.set(key(coordinate.x, coordinate.y), coordinate);
	if (state.framePending) return;
	state.framePending = true;
	requestAnimationFrame(async () => {
		state.framePending = false;
		const coordinates = [...state.stroke.values()];
		state.stroke.clear();
		if (coordinates.length) {
			state.strokePromise = queueDotNet(state, 'HandleCanvasStroke', coordinates);
			await state.strokePromise;
		}
	});
}

function queueDotNet(state, method, ...arguments_) {
	state.strokePromise = state.strokePromise
		.catch(error => console.error('Terrain Planner canvas operation failed.', error))
		.then(() => state.dotnet.invokeMethodAsync(method, ...arguments_));
	return state.strokePromise;
}

function queueStrokeLine(state, coordinate) {
	if (!coordinate) return;
	const start = state.lastPaintCoordinate || coordinate;
	let x = start.x;
	let y = start.y;
	const dx = Math.abs(coordinate.x - start.x);
	const sx = start.x < coordinate.x ? 1 : -1;
	const dy = -Math.abs(coordinate.y - start.y);
	const sy = start.y < coordinate.y ? 1 : -1;
	let error = dx + dy;
	while (true) {
		queueStroke(state, { x, y });
		if (x === coordinate.x && y === coordinate.y) break;
		const twiceError = 2 * error;
		if (twiceError >= dy) { error += dy; x += sx; }
		if (twiceError <= dx) { error += dx; y += sy; }
	}
	state.lastPaintCoordinate = coordinate;
}

function drawGrid(state, context, size, left, top, right, bottom) {
	context.strokeStyle = 'rgba(168, 190, 207, .16)';
	context.lineWidth = Math.max(1, state.ratio * .6);
	context.beginPath();
	for (let x = left; x <= right + 1; x++) {
		const screenX = state.panX * state.ratio + x * size;
		context.moveTo(screenX, state.panY * state.ratio + top * size);
		context.lineTo(screenX, state.panY * state.ratio + (bottom + 1) * size);
	}
	for (let displayY = top; displayY <= bottom + 1; displayY++) {
		const screenY = state.panY * state.ratio + displayY * size;
		context.moveTo(state.panX * state.ratio + left * size, screenY);
		context.lineTo(state.panX * state.ratio + (right + 1) * size, screenY);
	}
	context.stroke();
}

function render(state) {
	const context = state.context;
	const canvas = state.canvas;
	context.clearRect(0, 0, canvas.width, canvas.height);
	context.fillStyle = '#07111b';
	context.fillRect(0, 0, canvas.width, canvas.height);
	if (!state.width || !state.height) return;

	const size = state.tileSize * state.zoom * state.ratio;
	const left = Math.max(0, Math.floor((-state.panX * state.ratio) / size) - 1);
	const right = Math.min(state.width - 1, Math.ceil((canvas.width - state.panX * state.ratio) / size) + 1);
	const top = Math.max(0, Math.floor((-state.panY * state.ratio) / size) - 1);
	const bottom = Math.min(state.height - 1, Math.ceil((canvas.height - state.panY * state.ratio) / size) + 1);

	for (let displayY = top; displayY <= bottom; displayY++) {
		const y = state.height - 1 - displayY;
		for (let x = left; x <= right; x++) {
			const cell = state.cells.get(key(x, y));
			if (!cell) continue;
			const terrain = state.terrains.get(cell.terrainId);
			const screenX = state.panX * state.ratio + x * size;
			const screenY = state.panY * state.ratio + displayY * size;
			context.fillStyle = terrain?.colour || '#172431';
			context.fillRect(screenX, screenY, size, size);
			if (terrain?.text && size >= 20 * state.ratio) {
				context.fillStyle = '#f7fbff';
				context.font = `600 ${Math.min(14 * state.ratio, size * .38)}px ui-sans-serif, system-ui`;
				context.textAlign = 'center';
				context.textBaseline = 'middle';
				context.fillText(terrain.text, screenX + size / 2, screenY + size / 2, size * .75);
			}
			const visibleTags = cell.tagIds.slice(0, 4);
			const marker = Math.max(3 * state.ratio, Math.min(size * .18, 8 * state.ratio));
			visibleTags.forEach((id, index) => {
				context.fillStyle = state.tags.get(id)?.colour || '#f5a524';
				context.fillRect(screenX + size - marker * (index + 1) - 2 * state.ratio, screenY + size - marker - 2 * state.ratio, marker, marker);
			});
			if (cell.tagIds.length > 4 && size >= 26 * state.ratio) {
				context.fillStyle = '#ffffff';
				context.font = `700 ${9 * state.ratio}px ui-sans-serif, system-ui`;
				context.textAlign = 'right';
				context.textBaseline = 'bottom';
				context.fillText(`+${cell.tagIds.length - 4}`, screenX + size - 2 * state.ratio, screenY + size - marker - 3 * state.ratio);
			}
		}
	}

	drawGrid(state, context, size, left, top, right, bottom);
	if (state.selected) {
		const displayY = state.height - 1 - state.selected.y;
		context.strokeStyle = '#ffffff';
		context.lineWidth = 2 * state.ratio;
		context.strokeRect(state.panX * state.ratio + state.selected.x * size + state.ratio,
			state.panY * state.ratio + displayY * size + state.ratio, size - 2 * state.ratio, size - 2 * state.ratio);
	}

	context.fillStyle = 'rgba(7, 17, 27, .88)';
	context.fillRect(0, 0, canvas.width, 24 * state.ratio);
	context.fillRect(0, 0, 30 * state.ratio, canvas.height);
	context.fillStyle = '#91a8ba';
	context.font = `${10 * state.ratio}px ui-monospace, monospace`;
	context.textAlign = 'center';
	context.textBaseline = 'middle';
	for (let x = left; x <= right; x++) {
		context.fillText(String(x), state.panX * state.ratio + (x + .5) * size, 12 * state.ratio);
	}
	context.textAlign = 'right';
	for (let displayY = top; displayY <= bottom; displayY++) {
		const y = state.height - 1 - displayY;
		context.fillText(String(y), 25 * state.ratio, state.panY * state.ratio + (displayY + .5) * size);
	}
}

export function initialise(canvas, dotnet) {
	const state = {
		canvas,
		context: canvas.getContext('2d'),
		dotnet,
		width: 0,
		height: 0,
		cells: new Map(),
		terrains: new Map(),
		tags: new Map(),
		tileSize: 30,
		zoom: 1,
		panX: 42,
		panY: 34,
		ratio: 1,
		tool: 'paint',
		layer: 'terrain',
		painting: false,
		panning: false,
		spaceDown: false,
		lastPointer: null,
		lastPaintCoordinate: null,
		rectangleStart: null,
		selected: null,
		stroke: new Map(),
		strokePromise: Promise.resolve(),
		framePending: false,
		handlers: {}
	};

	state.handlers.pointerdown = event => {
		canvas.setPointerCapture(event.pointerId);
		const point = screenPoint(state, event);
		state.lastPointer = point;
		if (event.button === 1 || state.spaceDown) {
			state.panning = true;
			return;
		}
		if (event.button !== 0) return;
		const coordinate = mapPoint(state, point);
		if (!coordinate) return;
		state.selected = coordinate;
		state.dotnet.invokeMethodAsync('SelectCell', coordinate);
		if (state.tool === 'rectangle') {
			state.rectangleStart = coordinate;
		} else {
			state.painting = true;
			state.lastPaintCoordinate = coordinate;
			queueDotNet(state, 'BeginCanvasStroke');
			queueStroke(state, coordinate);
		}
		render(state);
	};
	state.handlers.pointermove = event => {
		const point = screenPoint(state, event);
		if (state.panning && state.lastPointer) {
			state.panX += (point.x - state.lastPointer.x) / state.ratio;
			state.panY += (point.y - state.lastPointer.y) / state.ratio;
			state.lastPointer = point;
			render(state);
			return;
		}
		state.lastPointer = point;
		if (state.painting && state.tool === 'paint' || state.painting && state.tool === 'erase') {
			queueStrokeLine(state, mapPoint(state, point));
		}
	};
	state.handlers.pointerup = event => {
		const point = screenPoint(state, event);
		const completedPaintStroke = state.painting;
		if (state.rectangleStart) {
			const end = mapPoint(state, point);
			if (end) state.dotnet.invokeMethodAsync('HandleCanvasRectangle', state.rectangleStart, end);
		}
		state.rectangleStart = null;
		state.painting = false;
		state.lastPaintCoordinate = null;
		if (completedPaintStroke) {
			requestAnimationFrame(() => {
				queueDotNet(state, 'EndCanvasStroke');
			});
		}
		state.panning = false;
	};
	state.handlers.wheel = event => {
		event.preventDefault();
		const point = screenPoint(state, event);
		const before = mapPoint(state, point);
		state.zoom = Math.max(.35, Math.min(4, state.zoom * (event.deltaY < 0 ? 1.12 : .89)));
		if (before) {
			const size = state.tileSize * state.zoom;
			state.panX = point.x / state.ratio - before.x * size - size / 2;
			state.panY = point.y / state.ratio - (state.height - 1 - before.y) * size - size / 2;
		}
		render(state);
	};
	state.handlers.keydown = event => {
		if (event.code === 'Space') { state.spaceDown = true; event.preventDefault(); }
		if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'z') {
			event.preventDefault();
			state.dotnet.invokeMethodAsync('HandleShortcut', event.shiftKey ? 'redo' : 'undo');
		}
		if ((event.ctrlKey || event.metaKey) && event.key.toLowerCase() === 'y') {
			event.preventDefault();
			state.dotnet.invokeMethodAsync('HandleShortcut', 'redo');
		}
		if (state.selected && ['ArrowLeft', 'ArrowRight', 'ArrowUp', 'ArrowDown'].includes(event.key)) {
			event.preventDefault();
			const next = { ...state.selected };
			if (event.key === 'ArrowLeft') next.x--;
			if (event.key === 'ArrowRight') next.x++;
			if (event.key === 'ArrowUp') next.y++;
			if (event.key === 'ArrowDown') next.y--;
			if (next.x >= 0 && next.x < state.width && next.y >= 0 && next.y < state.height) {
				state.selected = next;
				state.dotnet.invokeMethodAsync('SelectCell', next);
				render(state);
			}
		}
	};
	state.handlers.keyup = event => { if (event.code === 'Space') state.spaceDown = false; };
	state.handlers.contextmenu = event => event.preventDefault();

	for (const [name, handler] of Object.entries(state.handlers)) canvas.addEventListener(name, handler);
	state.resizeObserver = new ResizeObserver(() => resizeCanvas(state));
	state.resizeObserver.observe(canvas);
	instances.set(canvas, state);
	resizeCanvas(state);
}

export function setMap(canvas, model) {
	const state = instances.get(canvas);
	if (!state) return;
	state.width = model.width;
	state.height = model.height;
	state.cells = new Map(model.cells.map(cell => [key(cell.x, cell.y), cell]));
	state.terrains = new Map(model.terrains.map(terrain => [terrain.id, terrain]));
	state.tags = new Map(model.tags.map(tag => [tag.id, tag]));
	render(state);
}

export function updateCells(canvas, cells) {
	const state = instances.get(canvas);
	if (!state) return;
	for (const cell of cells) state.cells.set(key(cell.x, cell.y), cell);
	render(state);
}

export function setInteraction(canvas, layer, tool) {
	const state = instances.get(canvas);
	if (!state) return;
	state.layer = layer;
	state.tool = tool;
	state.canvas.style.cursor = tool === 'eyedropper' ? 'copy' : tool === 'erase' ? 'cell' : 'crosshair';
}

export function clearSelection(canvas) {
	const state = instances.get(canvas);
	if (!state) return;
	state.selected = null;
	render(state);
}

export function dispose(canvas) {
	const state = instances.get(canvas);
	if (!state) return;
	state.resizeObserver.disconnect();
	for (const [name, handler] of Object.entries(state.handlers)) canvas.removeEventListener(name, handler);
	instances.delete(canvas);
}
