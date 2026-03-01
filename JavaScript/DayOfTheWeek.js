/**
 * DayOfTheWeek.js
 * Prints today's name and ISO day number (Monday=1 ... Sunday=7).
 * Works in Node and browsers.
 */

function getDayInfo(date = new Date()) {
	const jsDay = date.getDay(); // 0 (Sun) - 6 (Sat)
	const isoDay = jsDay === 0 ? 7 : jsDay;
	const names = ['Monday','Tuesday','Wednesday','Thursday','Friday','Saturday','Sunday'];
	const name = names[isoDay - 1];
	const dayOfYear = getDayOfYear(date);
	return { name, isoDay, date, dayOfYear };
}

function getDayOfYear(date = new Date()) {
	const start = new Date(date.getFullYear(), 0, 1);
	const diff = date - start; // milliseconds since start of year
	return Math.floor(diff / (24 * 60 * 60 * 1000)) + 1;
}


function formatDayInfo(info) {
	return `Today is ${info.name} (day ${info.isoDay} of the week), day ${info.dayOfYear} of the year.`;
}

function showDayInfo() {
	const info = getDayInfo();
	const output = formatDayInfo(info);
	if (typeof document !== 'undefined' && document.body) {
		const el = document.createElement('div');
		el.textContent = output;
		document.body.appendChild(el);
	}
	if (typeof console !== 'undefined') console.log(output);
	return info;
}

// Expose a function named `DayOfTheWeek` as requested. This returns the
// formatted string and writes it into an element with id `day-output` if present.
function DayOfTheWeek() {
	const info = getDayInfo();
	const output = formatDayInfo(info);
	if (typeof document !== 'undefined' && document.getElementById) {
		const target = document.getElementById('day-output');
		if (target) {
			target.textContent = output;
		} else if (document.body) {
			const el = document.createElement('div');
			el.id = 'day-output';
			el.textContent = output;
			document.body.appendChild(el);
		}
	}
	if (typeof console !== 'undefined') console.log(output);
	return output;
}

if (typeof module !== 'undefined' && module.exports) {
	module.exports = { getDayInfo, showDayInfo };
	module.exports.DayOfTheWeek = DayOfTheWeek;
} else {
	showDayInfo();
	DayOfTheWeek();
}

